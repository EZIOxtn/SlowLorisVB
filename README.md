# SlowlorisVB

SlowlorisVB is a VB.NET-based console app tool designed to test website resilience by performing Slowloris-style attacks. It allows users to customize various parameters such as proxies, number of sockets, speed, and other configurations, providing control over the attack strategy for testing purposes.

## Features

- **Custom Proxies**: Users can set up their own proxies to route the requests.
- **Configurable Sockets**: Customize the number of sockets to manage the scale of the attack.
- **Speed Control**: Adjust the speed of the attack by controlling the delay between socket connections.
- **Advanced Configuration**: Full control over the configuration settings to tailor the attack strategy based on testing needs.

## How Does It Work

1. **Start via Command Line**:
   - Open your command prompt. CMD
   - Navigate to the directory where the `slowloris.exe` file is located using:
     ```bash
     cd slowloris.exe
     ```
  2 . **Usage**:
   ```bash
   SlowLoris <target> -p <port> [-x <proxy_file>] [-proxytimeout <proxy_timeout_seconds>] [-sleep <sleep_seconds>] [-n <num_sockets>]
 ```
## video exemple
[Watch this video](https://www.example.com/path-to-video.mp4)

### Prerequisites

- Windows 10 or higher
- .NET Framework 4.7.2 or higher
- VB.NET-compatible IDE (Visual Studio)

## Setup and Run

1. **Open the Solution**:
   - Navigate to the `SlowlorisVB` directory.
   - Open the solution in Visual Studio.

2. **Build the Application**:
   - In Visual Studio, build the solution to generate the executable file.

3. **Run the Application**:
   - Launch the application after the build completes. The main interface will allow you to configure the attack parameters.

## How It Works

1. **Set Proxies**:
   - Users can enter the list of proxies they want to use for routing traffic.

2. **Configure Number of Sockets**:
   - Select the number of sockets to be opened for the attack. The more sockets, the heavier the load on the target server.

3. **Adjust Speed**:
   - Control the speed of the attack by adjusting the delay between the opening of sockets.

4. **Start Test**:
   - Once configured, click the 'Start' button to initiate the Slowloris test.

## Technologies Used

- **VB.NET**: The core programming language used to develop the application.
- **.NET Framework**: Provides the base for running the application on Windows.
- **Windows Forms**: Used for creating the user interface.
- **TCP Protocol**: Handles the low-level socket connections for the attack.

## Future Enhancements

- **Enhanced Proxy Management**: Add support for proxy pools and automatic proxy rotation.
- **Log Output**: Create detailed logs for each test, showing success, failure, and errors.
## Disclaimer
This tool is created strictly for educational and testing purposes only. It should only be used to test the resilience of websites that you have explicit permission to test. Any misuse of this tool to attack websites without authorization is illegal and unethical. The creator is not responsible for any misuse or damage caused by the improper use of this tool.

