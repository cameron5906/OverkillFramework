# Overkill Robotics Framework

Overkill is a robotics platform built on top of .NET core. It provides a simple solution to building highly accessible telepresent robots that are controllable through a web browser on a desktop or smartphone.

For the Web service platform that manages Overkill-enabled robots, see https://github.com/cameron5906/OverkillWebService

### Assumptions

It is assumed that every Overkill robot has the following:

- A connection to the Internet, either by Wifi or 4G modem
- At least one video capture device (USB webcam, Picam, etc.)
- A supported communication interface, currently TCP or GPIO

### Architecture

The Overkill framework is comprised of the following components:

- Overkill.Core - Contains core interfaces such as IPlugin, IVehicle, communication protocols, positioning services, video transmission services, and core Topics.
- Overkill.Common - Contains enums representing supported protocols, positioning systems, and configuration models.
- Overkill.PubSub - Contains the PubSub service as well as interfaces for implementing your own Topics, middleware, and topic transformers
- Overkill.Services - Contains generalized services for audio, logging, and networking
- Overkill.Proxies - Contains proxies for use cases like file I/O, threading, child process management. This is mostly in place to allow for higher degrees of unit testing components.
- Overkill.Websockets - Contains the Websocket service and message handlers for interacting with the Overkill Web Platform.

### PubSub

To achieve the goal of a highly decoupled framework, I went with a Publish/Subscribe model for distributing various "Topics" throughout the core framework that allows for different components to interact with one-another in a non-reliant manner. With this model, we are also able to support Plugins that are capable of utilizing core topics, or even injecting their own and utilizing topics from other Plugins as well.

The PubSub mechanism of Overkill also allows for use of middleware to mutate topics before they reach their subscribers, or transformers, which are like middleware but instead of mutating the topic itself they instead mutate the original topic and then dispatch themselves separately, allowing the original topic to stay "pure."

### Communication Protocols

Currently Overkill supports TCP and GPIO protocols to instruct the connected robot to do tasks. The framework is agnostic toward these protocols, meaning it is very simple to add new ones by simply implementing your own ICommunicationPayload, IConnectionInitializer, and IConnectionInterface. The specific protocol is selected while Overkill boots by checking the configuration.json file.

### Positioning

Overkill has a built-in concept of positioning - whether that is local environment or perhaps some form of GPS. Currently, Overkill has built in support for Quectel-CM modem devices which allows for it to read from the serial interface and parse out latitude and longitude data which is then published using PubSub to any component that may be interested in the robot's location.

Adding a different positioning mechanism or separate GPS driver is as simple as implementing your own IPositioningService and specifying it in the configuration.json file.

### Video Transmission

A built-in implementation of FFmpeg video transmission is included in Overkill. Different implementations can be added by simply inheriting the IVideoTransmissionService, which expects you to launch a process that sends the requested video device output to a remote endpoint specified in configuration.json. This service is also expected to subscribe to the ChangeVideoSource topic for instances where a user may want to see a different view from another capture device.

### Plugins

Overkill has support for plugin assemblies built on .NET core. By simply specifying the Plugin DLL name in the working directory, Overkill will inject it during its boot procedure and initialize it with the same DI service provider that it uses internally, allowing the Plugin to utilize the services inside of the Core framework, including PubSub.

### Robot/Vehicle Drivers

Since the Overkill Framework is built to support any type of robot, the "drivers" for the robot itself are also a Plugin of sorts. To boot Overkill, you need to specify a driver assembly. This assembly is loaded during the boot procedure and is expected to subscribe to specific topics, such as Drive, and use those to effectively send data to the robot via whichever communication protocol is supported.

To create a driver, simply add a project reference to Overkill.Core and Overkill.PubSub, have the main class inherit IVehicle, and implement the Initialization method. An IConnectionInterface is able to be injected into the IVehicle class which represents the communication channel to send instructions to the connected robot.



# Included Examples

### Plugin.Lidar

A plugin that is responsible for interfacing with an RPLidar A1M8 lidar scanner. This plugin contains an embedded Python script that is written to disk when Overkill boots up and initializes a UDP socket that the Python script sends measurement data to. The plugin then takes these 360 scans and dispatches a topic for components that are interested (and also reference Plugin.Lidar.)

### Vehicle.Traxxas

A vehicle driver for a Traxxas R/C car with connected WiFi receiver. This driver subscribes to the Drive topic and transforms it into a TraxxasInputMessage that is then sent to the car's WiFi receiver in a binary payload it expects.



# Future

### Accessories

An accessory is the concept of an external, physical device that is enclosed within the robot along with the device running the Overkill framework. Overkill should be able to expose a socket interface that allows for these devices to authenticate, become authorized, and begin sending additional data via PubSub to components loaded into the runtime assembly.

For example, a device that has the sole purpose of computing computer vision models and sending raw labeled data to a Plugin that is loaded into Overkill and knows how to consume said information to make driving decisions.