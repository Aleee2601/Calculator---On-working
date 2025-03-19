# WPF Calculator

This project is a WPF-based Calculator application developed in C# that emulates the standard Windows Calculator, with additional functionality inspired by the Programmer mode. The application supports both Standard and Programmer modes, along with a range of arithmetic and memory functions.

## Features

### Arithmetic Operations
- Perform basic operations such as addition, subtraction, multiplication, division, modulus, square root, square, sign change (+/-), and reciprocal (1/x).

### Memory Functions
Includes functionalities like:
- **MC:** Memory Clear
- **M+:** Memory Add
- **M-:** Memory Subtract
- **MS:** Memory Store
- **MR:** Memory Recall
- **M>:** Display the memory stack for selection in further operations

### User Input
- Supports both mouse clicks and keyboard inputs.
- Provides cut, copy, and paste capabilities using custom string manipulation (without default text control methods).

### Digit Grouping
- Numbers are formatted with digit grouping according to the user's regional settings (e.g., 1,000 in the UK vs. 1.000 in Romania).

### Programmer Mode
- Allows conversion between numeral systems (Binary, Octal, Decimal, and Hexadecimal).
- Only accepts valid digits according to the selected numeral base.
- Although conversions are displayed in various bases, calculations are performed in base 10.

### Persistent Settings
- The last used mode (Standard or Programmer), digit grouping option, and numeral base are saved and restored on subsequent runs.

### User Interface Enhancements
- Non-editable display for output.
- Menu options for File operations (Cut, Copy, Paste, Digit Grouping) and a Help/About section.
- The application window is non-resizable.

## Project Structure

- **CalculatorViewModel.cs:**  
  Contains the ViewModel that handles the calculator’s logic, data binding, and command implementations for user interactions.

- **CalculatorModel.cs:**  
  Provides the core mathematical operations and memory management functions.

- **MainWindow.xaml & MainWindow.xaml.cs:**  
  Define the user interface using XAML and handle UI-specific logic such as base conversions, button enabling/disabling for programmer mode, and keyboard input management.

## Requirements

- [.NET Framework](https://dotnet.microsoft.com/) (compatible with the version used during development)
- Visual Studio (or any other preferred C# IDE)

## How to Build and Run

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/your-repo-name.git
2. **Open the Solution:** Open the project in Visual Studio.

3. **Build the Project:** Build the solution to restore NuGet packages and compile the project.

4. **Run the Application:** Start the application by pressing F5 (Debug mode) or Ctrl + F5 (without debugging).
## Usage

### Standard Mode
- Use the standard set of arithmetic operations and functions as you would with a typical calculator.

### Programmer Mode
- Switch to Programmer mode using the provided menu or toggle button. In this mode, you can:
  - Convert between numeral systems.
  - Enter only valid digits for the selected base (2, 8, 10, or 16).
  - View conversion outputs for Hex, Dec, Oct, and Bin.

### Persistent Settings
- The application remembers your last settings, such as the selected mode, digit grouping preference, and numeral base, even after closing and reopening the app.

### About
- This application was developed by **Alexandra Onose**,  as part of the assignment for **Advanced programming methods** class.

