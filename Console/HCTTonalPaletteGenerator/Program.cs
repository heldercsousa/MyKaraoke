using MyVocaList.Console.HCTTonalPaletteGenerator;

var hctTonalPalette = MaterialColorPaletteGenerator.GenerateXamlResourceDictionary();
var path = $"HCTTonalPalette_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xaml";
File.WriteAllText(path, hctTonalPalette);
Console.WriteLine("HCT Tonal Palette XAML resource dictionary generated.");