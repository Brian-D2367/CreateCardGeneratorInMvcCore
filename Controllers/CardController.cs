using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using CreateCards.Models;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using System.Drawing.Imaging;
using System.Xml.Linq;
using static CreateCards.Controllers.CardController;


namespace CreateCards.Controllers
{
    public class CardController : Controller
    {
        public class DealerCarDetails
        {
            public DealerDetails Dealer { get; set; }
            public CarDetails Car { get; set; }
            public List<string> CarAssets { get; set; }
            public string Watermark { get; set; }
        }

        public class DealerDetails
        {
            public string Name { get; set; }
            public string ImgUrl { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Address1 { get; set; }
        }

        public class CarDetails
        {
            public string CarName { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string Price { get; set; }
            public string Colour { get; set; }
            public string RegistrationNumber { get; set; }
            public string Mileage { get; set; }
            public string FuelType { get; set; }
            public string Owner { get; set; }
        }


        // GET: CardController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult GenerateCertificates()
        {
            var certificateDataList = new DealerCarDetails
            {
                Dealer = new DealerDetails
                {
                    Name = "Deepak Yadav",
                    ImgUrl = "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\profile.JPG",
                    PhoneNumber = "98976543128",
                    Email = "info@bestcars.com",
                    Address = "G107, Near Dreams Mall",
                    Address1 = "Bhandup East, Mumbai 400042"
                },
                Car = new CarDetails
                {
                    CarName = "Jaguar F-Pace",
                    Brand = "Best",
                    Model = "X1",
                    Price = "25,0000",
                    RegistrationNumber = "MH01",
                    FuelType = "Petrol",
                    Colour = "White",
                    Mileage = "38,000 KMs",
                    Owner = "2"
                },
                CarAssets = new List<string>
                    {
                        "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\car1.jpeg",
                        "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\car2.jpeg",
                        "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\car3.jpeg",
                        "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\car4.jpeg"
                    },
                Watermark = "BestCars Inc."
            };

            var carTemplate = "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\template.png";
            var carDetailsTemplate = "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\template.png";
            var dealerDetailsTemplate = "C:\\Deepak\\LionBridge\\POC\\CreateCards\\wwwroot\\images\\dealer.png";

            var certificateImages = new List<string>();

            if (certificateDataList.Dealer != null)
            {
                var dealerCertificate = GenerateDealerCertificate(certificateDataList, dealerDetailsTemplate);
                certificateImages.Add(dealerCertificate);
            }
            
            if (certificateDataList.Car != null)
            {
                var carCertificate = GenerateCarCertificate(certificateDataList, carDetailsTemplate);
                certificateImages.Add(carCertificate);
            }
            if (certificateDataList.CarAssets.Count > 0)
            {
                foreach (var assetUrl in certificateDataList.CarAssets)
                {
                    var assetCertificate = GenerateAssetCertificate(assetUrl, certificateDataList.Watermark, carTemplate);
                    certificateImages.Add(assetCertificate);
                }
            }

            SaveCertificateImages(certificateImages);
            return Ok(certificateImages);
        }

        private string GenerateDealerCertificate(DealerCarDetails certificateDataList, string template)
        {
            using (var templateImage = LoadImage(template))
            {
                var graphics = Graphics.FromImage(templateImage);
                DrawDealerDetails(graphics, certificateDataList.Dealer);
                DrawWatermark(graphics, certificateDataList.Watermark);

                return ConvertImageToBase64(templateImage);
            }
        }

        private string GenerateCarCertificate(DealerCarDetails certificateDataList, string template)
        {
            using (var templateImage = LoadImage(template))
            {
                var graphics = Graphics.FromImage(templateImage);
                DrawCarDetails(graphics, certificateDataList.Car);
                DrawWatermark(graphics, certificateDataList.Watermark);

                return ConvertImageToBase64(templateImage);
            }
        }

        private string GenerateAssetCertificate(string assetUrl, string watermark, string template)
        {
            using (var templateImage = LoadImage(template))
            {
                var graphics = Graphics.FromImage(templateImage);
                DrawWatermark(graphics, watermark);
                DrawImageOnTemplate(graphics, assetUrl);

                return ConvertImageToBase64(templateImage);
            }
        }

        private Image LoadImage(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException($"Image not found at path: {path}");

            return Image.FromFile(path);
        }

        private void DrawDealerDetails(Graphics graphics, DealerDetails dealer, int margin = 25)
        {
            // Font for the name (bold)
            var boldFont = new Font("Arial", 20, FontStyle.Bold);
            var font = new Font("Arial", 15);

            // Brush for the phone number (red)
            var yellowBrush = new SolidBrush(Color.Gold);  // Changed brush color to yellow

            // Black brush for other text
            var blackBrush = Brushes.Black;

            // Text width (estimate for centering)
            var nameWidth = graphics.MeasureString(dealer.Name, boldFont).Width;
            var phoneWidth = graphics.MeasureString(dealer.PhoneNumber, font).Width;
            var addressWidth = graphics.MeasureString(dealer.Address.Replace("\n", "\n "), font).Width;
            var address1Width = graphics.MeasureString(dealer.Address1.Replace("\n", "\n "), font).Width;

            // Calculate combined address width
            var combinedAddressWidth = Math.Max(addressWidth, address1Width);

            // Calculate X-coordinate for right alignment (considering margin)
            var maxWidth = Math.Max(nameWidth, Math.Max(phoneWidth, combinedAddressWidth));
            var xCoordinate = 700 - margin - maxWidth;

            // Draw the name with bold font and yellow brush
            graphics.DrawString(dealer.Name, boldFont, yellowBrush, xCoordinate, 100);

            // Draw the phone number with yellow brush
            graphics.DrawString(dealer.PhoneNumber, font, yellowBrush, xCoordinate, 150);

            // Draw the address with yellow brush and line breaks
            graphics.DrawString(dealer.Address.Replace("\n", "\n "), font, yellowBrush, xCoordinate, 200);

            // Draw the address1 with yellow brush and line breaks
            graphics.DrawString(dealer.Address1.Replace("\n", "\n "), font, yellowBrush, xCoordinate, 250);
        }




        private void DrawImageOnTemplate(Graphics graphics, string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl) && System.IO.File.Exists(imageUrl))
            {
                using (var userImage = Image.FromFile(imageUrl))
                {
                    var imageSize = new Size(600, 300); // Adjust size as needed
                    var imagePosition = new PointF((700 - imageSize.Width) / 2, (399 - imageSize.Height) / 2);
                    graphics.DrawImage(userImage, new RectangleF(imagePosition, imageSize));
                }
            }
        }

        private void DrawCarDetails(Graphics graphics, CarDetails car)
        {
            var font = new Font("Arial", 20);
            var boldFont = new Font(font, FontStyle.Bold);
            var brush = Brushes.Black;
            var redBrush = Brushes.Red;
            var greyBrush = Brushes.Gray;
            var darkBlackBrush = Brushes.Black;
            var margin = 20; // Adjust margin as needed
            var separator = ":   "; // Adjust spacing as needed

            var textSize = graphics.MeasureString("Car Details", font);
            var centerX = (graphics.VisibleClipBounds.Width - textSize.Width) / 2;
            var yPos = margin;

            yPos += (int)textSize.Height + 10;
            graphics.DrawString("Car Details", font, brush, margin, yPos, StringFormat.GenericDefault); // Align left

            yPos += (int)textSize.Height + 10;

            // Draw Car Name (bold dark black)
            var carNameSize = graphics.MeasureString(car.CarName, boldFont);
            graphics.DrawString(car.CarName, boldFont, darkBlackBrush, margin, yPos, StringFormat.GenericDefault); // Align left

            yPos += (int)carNameSize.Height + 15;

            // Calculate the width for each side (left and right)
            var boxWidth = (graphics.VisibleClipBounds.Width - 2 * margin) / 2;

            var leftXPos = margin;
            var labelWidth = graphics.MeasureString("Price", boldFont).Width;
            graphics.DrawString("Price", boldFont, darkBlackBrush, leftXPos, yPos);
            graphics.DrawString($"{separator}{car.Price}", font, greyBrush, leftXPos + labelWidth, yPos);

            var rightXPos = margin + boxWidth;
            labelWidth = graphics.MeasureString("Colour", boldFont).Width;
            graphics.DrawString("Colour", boldFont, darkBlackBrush, rightXPos, yPos);
            graphics.DrawString($"{separator}{car.Colour}", font, greyBrush, rightXPos + labelWidth, yPos);

            yPos += (int)textSize.Height + 10;

            leftXPos = margin;
            labelWidth = graphics.MeasureString("Reg.No", boldFont).Width;
            graphics.DrawString("Reg.No", boldFont, darkBlackBrush, leftXPos, yPos);
            graphics.DrawString($"{separator}{car.RegistrationNumber}", font, greyBrush, leftXPos + labelWidth, yPos);

            rightXPos = margin + boxWidth;
            labelWidth = graphics.MeasureString("Mileage", boldFont).Width;
            graphics.DrawString("Mileage", boldFont, darkBlackBrush, rightXPos, yPos);
            graphics.DrawString($"{separator}{car.Mileage}", font, greyBrush, rightXPos + labelWidth, yPos);

            yPos += (int)textSize.Height + 10;

            leftXPos = margin;
            labelWidth = graphics.MeasureString("Fuel", boldFont).Width;
            graphics.DrawString("Fuel", boldFont, darkBlackBrush, leftXPos, yPos);
            graphics.DrawString($"{separator}{car.FuelType}", font, greyBrush, leftXPos + labelWidth, yPos);

            rightXPos = margin + boxWidth;
            labelWidth = graphics.MeasureString("Owner", boldFont).Width;
            graphics.DrawString("Owner", boldFont, darkBlackBrush, rightXPos, yPos);
            graphics.DrawString($"{separator}{car.Owner}", font, greyBrush, rightXPos + labelWidth, yPos);
        }


        private void DrawWatermark(Graphics graphics, string watermarkText)
        {
            var watermarkFont = new Font("Arial", 12);
            var watermarkBrush = new SolidBrush(Color.Gold); // Semi-transparent red
            var watermarkSize = graphics.MeasureString(watermarkText, watermarkFont);
            var watermarkPosition = new PointF(700 - watermarkSize.Width - 20, 399 - watermarkSize.Height - 20);
            graphics.DrawString(watermarkText, watermarkFont, watermarkBrush, watermarkPosition);
        }



        private string ConvertImageToBase64(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private void SaveCertificateImages(List<string> certificateImages)
        {
            foreach (var certificateImage in certificateImages)
            {
                SaveImage(certificateImage);
            }
        }




        // Method to save the image to a file
        private void SaveImage(string base64Image)
        {
            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Image);

            // Directory to save the generated images
            string saveDirectory = @"C:\Deepak\LionBridge\POC\CreateCards\GeneratedImages\";

            // Generate a unique file name for the image
            string fileName = $"certificate_{DateTime.Now.Ticks}.png";
            string filePath = Path.Combine(saveDirectory, fileName);

            try
            {
                // Save the image to file
                System.IO.File.WriteAllBytes(filePath, imageBytes);
                Console.WriteLine($"Image saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                // Optionally, handle the exception or log the error
            }
        }
    }
}
