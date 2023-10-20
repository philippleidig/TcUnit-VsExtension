![TcUnit logo](https://github.com/philippleidig/TcUnit-VsExtension/blob/main/assets/TcUnit.png)
[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/philippleidig/TcUnit-VsExtension/blob/main/LICENSE)
[![Open Source? Yes!](https://badgen.net/badge/Open%20Source%20%3F/Yes%21/blue?icon=github)](https://github.com/philippleidig/TcUnit-VsExtension)

# TcUnit - Visual Studio Extension
Visual Studio Extension for higher productivity when using [TcUnit](https://github.com/tcunit/TcUnit) for unit testing with [TwinCAT 3.1](https://www.beckhoff.com/twincat3/) from [Beckhoff](https://www.beckhoff.com).

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)

## Installation

To install the TcUnit Visual Studio Extension, follow these steps:

1. [Download](https://github.com/philippleidig/TcUnit-VsExtension/releases/latest) the latest installer.
3. Select the Visual Studio and TcXaeShell versions, which the TcUnit Extension should be installed for.
4. Click on "Install".
5. Follow the on-screen instructions to complete the installation.

## Usage

### Add Test Suite

![add test suite command](./assets/images/add-test-suite-command.png)

![add test suite result](./assets/images/add-test-suite-result.png)

#### Add Test Case

![add test case command](./assets/images/add-test-case-command.png)

![add test case result](./assets/images/add-test-case-result.png)

![add test case template](./assets/images/add-test-case-template.png)

## Visual Studio Test Explorer Support

> [!NOTE]  
> VS Test Explorer currently not supported due to open issue
> see [TcUnit #218](https://github.com/tcunit/TcUnit/issues/218) for more information

![vs-test-explorer](/assets/images/vs-test-explorer.png)

Uses [TcUnit.TestAdapter](https://github.com/philippleidig/TcUnit-VsTestAdapter) for extending [vstest](https://github.com/microsoft/vstest)

## Options

Configure test suite and test case naming convention using regular expressions and provide the test case template source code in the options.

![vs-options](/assets/images/vs-options.png)

## PLC Project Template

1. Template is copied to "C:\TwinCAT\3.1\Components\Plc\PlcTemplates\1.0.0.0\Plc Templates\TcUnit" after installation
2. Add "TcUnit Template" as PLC project

![add tcunit template](./assets/images/add-tcunit-plc-template.png)

3. Basic setup for unit testing is already included in the template

![tcunit template](./assets/images/tcunit-plc-template.png)

## License
[MIT](./LICENSE.MD)