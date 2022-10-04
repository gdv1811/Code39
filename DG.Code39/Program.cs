using DG.Code39;

var text = Console.ReadLine();

var code39 = new Code39();
var barcode = code39.Encode(text);
Console.WriteLine($"Barcode: {barcode}");

var decodedText = code39.Decode(barcode + "010");
Console.WriteLine($"Decoded: {decodedText}");

decodedText = code39.Decode(Code39.Reverse(barcode));
Console.WriteLine($"Decoded: {decodedText}");
