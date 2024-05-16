
using Adroit_v8.Model;
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.CustomerCentric;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Aes = System.Security.Cryptography.Aes;

namespace Adroit_v8.Config
{
    public static class Helper
    {
        public static Dictionary<decimal, decimal> GetTotalLoanAmount(decimal amount, decimal interestRate)
        {
            var res = new Dictionary<decimal, decimal>();
            decimal newInt = Math.Round((interestRate / 100) * amount, 2);
            decimal total = Math.Round((amount + newInt), 2);
            res.Add(newInt, total);
            return res;
        }
        public static string ConvertIFormFilesToBase64(IFormFile file)
        {
            string val = "";
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    val = Convert.ToBase64String(fileBytes);
                }
            }
            return val;
        }
        public static string ConvertFromPathToBase64(string filePath)
        {
            string val = "";
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                val = Convert.ToBase64String(fileBytes);
            }
            catch (FileNotFoundException)
            {
                val = "File not found at the specified path.";
            }
            catch (Exception ex)
            {
                val = "An error occurred: " + ex.Message;
            }
            return val;
        }
        public static void ProcessFileUpload(IFormFile fileRealName, string FileNameGuild, string fileName, string SavePath)
        {
            string path = Path.Combine(SavePath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fil = string.Empty;
            var fileexetension = string.Empty;

            fil = $"{Path.Combine(path, fileName)}";
            var f1 = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            fileRealName.CopyTo(f1);
            f1.Close();
        }
        public class DataEncryption
        {
            public async Task<byte[]> EncryptAsync(string clearText, string passphrase)
            {
                using Aes aes = Aes.Create();
                aes.Key = DeriveKeyFromPassword(passphrase);
                aes.IV = IV;

                using MemoryStream output = new();
                using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

                await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
                await cryptoStream.FlushFinalBlockAsync();

                return output.ToArray();
            }
            public async Task<string> DecryptAsync(byte[] encrypted, string passphrase)
            {
                using Aes aes = Aes.Create();
                aes.Key = DeriveKeyFromPassword(passphrase);
                aes.IV = IV;

                using MemoryStream input = new(encrypted);
                using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

                using MemoryStream output = new();
                await cryptoStream.CopyToAsync(output);

                return Encoding.Unicode.GetString(output.ToArray());
            }
            private static byte[] IV =
            {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };
            private static byte[] DeriveKeyFromPassword(string password)
            {
                var emptySalt = Array.Empty<byte>();
                var iterations = 1000;
                var desiredKeyLength = 16; // 16 bytes equal 128 bits.
                var hashMethod = HashAlgorithmName.SHA384;
                return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                                 emptySalt,
                                                 iterations,
                                                 hashMethod,
                                                 desiredKeyLength);
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            string emailRegex = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

            email = email.Trim();
            var result = Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
            return result;
        }
        public static string RootPath()
        {
            return (string)AppDomain.CurrentDomain.GetData("ContentRootPath") ?? string.Empty;
        }

        public static List<LoanRestructingResponse> GetList(List<RegularLoanRestructure> lstOfCusId, List<Customer> allSav)
        {
            List<LoanRestructingResponse> neRes = new();
            foreach (var item in lstOfCusId)
            {
                var cus = allSav.FirstOrDefault(o => o.Id == item.CustomerId);
                if (cus != null)
                {
                    LoanRestructingResponse c = new();
                    c.PhoneNumber = cus.PhoneNumber;
                    c.CustomerRef = cus.CustomerRef;
                    c.LoanApplicationId = item.LoanApplicationId;
                    c.InitialTenorValue = item.InitialTenorValue;
                    c.InitialTenorId = item.InitialTenorId;
                    c.Bvn = cus.Bvn;
                    c.EmailAddress = cus.EmailAddress;
                    c.FirstName = cus.FirstName;
                    c.MiddleName = cus.MiddleName;
                    c.LastName = cus.LastName;
                    c.DateOfBirth = cus.DateOfBirth;
                    c.TenorId = item.TenorId;
                    c.TenorValue = item.TenorValue;
                    c.LoanAmount = item.LoanAmount;
                    c.DateSubmitted = item.DateCreated;
                    c.Status = item.Status;
                    neRes.Add(c);
                }
            }
            return neRes;
        }
        public static List<LoanTopUpResponse> GetList(List<LoanTopUp> lstOfCusId, List<Customer> allSav, List<Model.Gender> genders)
        {
            List<LoanTopUpResponse> neRes = new();
            foreach (var item in lstOfCusId)
            {
                var main = (from p in allSav
                            join g in genders on p.GenderId != null ? p.GenderId : 5 equals g.Id
                            where p.Id == item.CustomerId
                            select new
                            {
                                genderName = g.Name,
                                genderId = g.Id,
                                phone = p.PhoneNumber,
                                cusRef = p.CustomerRef,
                                bvn = p.Bvn,
                                EmailAddress = p.EmailAddress,
                                FirstName = p.FirstName,
                                MiddleName = p.MiddleName,
                                LastName = p.LastName,
                                DateOfBirth = p.DateOfBirth,
                                status = item.Status,

                            }).ToList();

                var cus = main.Count > 0 ? main.FirstOrDefault() : null;
                if (cus != null)
                {
                    LoanTopUpResponse c = new();
                    c.PhoneNumber = cus.phone;
                    c.CustomerRef = cus.cusRef;
                    c.LoanApplicationId = item.LoanApplicationId;
                    c.CurrentLoanApplicationId = item.CurrentLoanApplicationId;
                    c.Comment = item.Comment;
                    c.StatusName = item.StatusName;
                    c.NewLoanTopUpTenor = item.NewLoanTopUpTenor;
                    c.NewLoanTopUpAmount = item.NewLoanTopUpAmount;
                    c.Bvn = cus.bvn;
                    c.PreviousLoanBalance = item.PreviousLoanBalance;
                    c.Tenor = item.LoanDurationValue;
                    c.Gender = cus.genderName;
                    c.NewLoanAmount = item.NewLoanAmount;
                    c.TopUpAmount = item.TopUpAmount;
                    c.EmailAddress = cus.EmailAddress;
                    c.FirstName = cus.FirstName;
                    c.MiddleName = cus.MiddleName;
                    c.LastName = cus.LastName;
                    c.DateOfBirth = cus.DateOfBirth;
                    c.LoanAmount = item.LoanAmount;
                    c.InitialLoanAmount = item.InitialLoanAmount;
                    c.DateSubmitted = item.DateCreated;
                    neRes.Add(c);
                }
            }
            return neRes.OrderByDescending(o => o.DateSubmitted).ToList();
        }

        public static string ReadFromText(string payload)
        {
            string value = "";
            try
            {
                value = File.ReadAllText(payload);
                File.Delete(payload);
                return value;
            }
            catch (Exception e)
            {
                e.ToString();
                return value;
            }
        }
        public static void WriteToText(string payload, string location)
        {
            try
            {
                string filepath = System.IO.Path.Combine(RootPath(), $"TempOTP/");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + ".txt";
                if (!File.Exists(location))
                {
                    File.Create(location).Dispose();
                }
                using StreamWriter sw = File.AppendText(location);
                sw.WriteLine(payload);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        public static void InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Services
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddScoped(typeof(ISSMongoRepository<>), typeof(SSMongoRepository<>));
            services.AddScoped(typeof(ICustomerCentricRepository<>), typeof(CustomerCentricRepository<>));
            services.AddScoped(typeof(IAdroitRepository<>), typeof(AdroitRepository<>));
            services.AddSingleton(configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>());
        }
        public static List<ScheduleResponse> getScheduleAndAmount(decimal interestRate, decimal loanAmount, int duration)
        {
            List<ScheduleResponse> resList = new();
            var presentDate = DateTime.Now.Date;
            var totalToRefund = Math.Abs((Math.Abs(interestRate / 100) * loanAmount) + loanAmount);
            var totalToRefundPerMonths = Math.Abs(totalToRefund / duration);
            for (int i = 1; i <= duration; i++)
            {
                ScheduleResponse res = new();
                res.Amount = totalToRefundPerMonths;
                res.RepaymentSchedule = presentDate.AddMonths(i).ToString();
                resList.Add(res);
            }
            return resList;
        }

        public static DataTable ConvertExcelToDatatable(IFormFile file)
        {
            DataTable table = new DataTable();
            using (var stream = file.OpenReadStream())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                ExcelPackage package = new ExcelPackage();
                package.Load(stream);
                if (package.Workbook.Worksheets.Count > 0)
                {
                    using (ExcelWorksheet workSheet = package.Workbook.Worksheets.First())
                    {
                        int noOfCol = workSheet.Dimension.End.Column;
                        int noOfRow = workSheet.Dimension.End.Row;
                        int rowIndex = 1;

                        for (int c = 1; c <= noOfCol; c++)
                        {
                            table.Columns.Add(workSheet.Cells[rowIndex, c].Text);
                        }
                        rowIndex = 2;
                        for (int r = rowIndex; r <= noOfRow; r++)
                        {
                            DataRow dr = table.NewRow();
                            for (int c = 1; c <= noOfCol; c++)
                            {
                                dr[c - 1] = workSheet.Cells[r, c].Value;
                            }
                            table.Rows.Add(dr);
                        }

                        return table;
                    }
                }
                else
                    return table;

            }
        }
        public class EntityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, object> keySelector;

            public EntityComparer(Func<T, object> keySelector)
            {
                this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            }

            public bool Equals(T x, T y)
            {
                object keyX = keySelector(x);
                object keyY = keySelector(y);

                return keyX.Equals(keyY);
            }

            public int GetHashCode(T obj)
            {
                object key = keySelector(obj);
                return key.GetHashCode();
            }
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static string ConvertIFormFilesToZip(List<IFormFile> formFiles)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var formFile in formFiles)
                    {
                        // Create a new entry in the zip archive for each file
                        ZipArchiveEntry entry = zipArchive.CreateEntry(formFile.FileName);

                        // Write the contents of the IFormFile to the zip entry
                        using Stream entryStream = entry.Open();
                        formFile.CopyTo(entryStream);
                    }
                }

                var res = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(res);
                return base64String;
            }
        }
        public static List<string> ConvertIFormFilesToBase64(List<IFormFile> formFiles)
        {
            List<string> base64Strings = new();

            foreach (var formFile in formFiles)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    formFile.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    string base64String = Convert.ToBase64String(fileBytes);
                    base64Strings.Add(base64String);
                }
            }

            return base64Strings;
        }
        public static bool FileFormatValidation(IFormFile postedFile)
        {
            bool fileCheck = false;
            string fileExtension = Path.GetExtension(postedFile.FileName);

            switch (fileExtension.ToLower())
            {
                case ".jpg":
                    fileCheck = true;
                    break;
                case ".png":
                    fileCheck = true;
                    break;
                case ".doc":
                    fileCheck = true;
                    break;
                case ".jpeg":
                    fileCheck = true;
                    break;
                default:
                    fileCheck = false;
                    break;
            }
            return fileCheck;
        }
    }
}
