using DG.Code39;

var text = Console.ReadLine();

var code39 = new Code39();
var barcode = code39.Encode(text);
Console.WriteLine($"Barcode: {barcode}");

var decodedText = code39.Decode(barcode);
Console.WriteLine($"Decoded: {decodedText}");

decodedText = code39.Decode(Reverse(barcode));
Console.WriteLine($"Decoded: {decodedText}");


static string Reverse(string s)
{
    char[] charArray = s.ToCharArray();
    Array.Reverse(charArray);
    return new string(charArray);
}