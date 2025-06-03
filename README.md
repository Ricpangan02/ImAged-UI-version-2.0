# ImAged

**ImAged** is a WPF desktop application for image viewing and basic image processing, featuring a modern UI with Material Design styles.

## Features

- **Custom Title Bar** with app icon and window controls
- **Image Import** (`.ttl` files)
- **Image Transformations:** Rotate, Flip (Horizontal/Vertical)
- **Image Adjustments:** Brightness, Contrast, Saturation
- **Image Filters:** Grayscale, Sepia, Invert, Reset
- **File List** with status indicators
- **Tabbed Main Area:** Files, Activity Log, Validation Results
- **Material Design-inspired UI**

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Windows 10/11

## Getting Started

1. **Clone or Download the Repository**

2. **Add Your App Icon**

   - Place your icon file as `Assets/LogoImAged.png` (for the custom title bar) and/or `Assets/LogoImAged.ico` (for the taskbar icon).
   - If you want a crisp taskbar icon, use a `.ico` file and set in `MainWindow.xaml`:
     ```xml
     Icon="Assets/LogoImAged.ico"
     ```

3. **Restore NuGet Packages**
   ```
   dotnet restore ImAged.csproj
   ```

4. **Build the Project**
   ```
   dotnet build ImAged.csproj
   ```

5. **Run the Application**
   ```
   dotnet run --project ImAged.csproj
   ```

## Project Structure

```
UIProject/
├── Assets/
│   ├── LogoImAged.png
│   └── LogoImAged.ico
├── App.xaml
├── App.xaml.cs
├── ImAged.csproj
├── MainWindow.xaml
└── MainWindow.xaml.cs
```

## Customization

- **App Icon:**  
  - For the taskbar, use a `.ico` file with multiple sizes.
  - For the custom title bar, the PNG is displayed via an `<Image>` in the XAML.
- **UI Styles:**  
  - Styles are defined in `MainWindow.xaml` under `<Window.Resources>`.
- **Image Processing Logic:**  
  - Extend or modify in `MainWindow.xaml.cs`.

## Troubleshooting

- **Icon Not Showing:**  
  - Ensure the icon file exists at the correct path and is included as a resource in the `.csproj`.
- **Build Errors:**  
  - Check for missing files or incorrect resource paths.
- **UI Not Loading:**  
  - Ensure all XAML tags are properly closed and event handlers are defined in code-behind.

## License

This project is for educational/demo purposes.  
Feel free to modify and use as needed.

---
