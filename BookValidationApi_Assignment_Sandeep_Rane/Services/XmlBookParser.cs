using BookValidationApi_Assignment_Sandeep_Rane.Models;
using System.Xml;
using System.Xml.Linq;

namespace BookValidationApi_Assignment_Sandeep_Rane.Services
{
    public sealed class XmlBookParser
    {
        private readonly string _xmlFilePath;

        public XmlBookParser(string xmlFilePath)
        {
            _xmlFilePath = xmlFilePath;
        }

        public BookValidationResult ParseBooks()
        {
            var result = new BookValidationResult();

            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true,
                DtdProcessing = DtdProcessing.Ignore
            };

            try
            {
                using var fileStream = File.OpenRead(_xmlFilePath);
                using var reader = XmlReader.Create(fileStream, settings);

                while (reader.ReadToFollowing("book"))
                {
                    string outerXml;
                    try
                    {
                        outerXml = reader.ReadOuterXml();
                    }
                    catch (XmlException)
                    {
                        result.InvalidBooks.Add(new InvalidBookEntry
                        {
                            Title = "(unknown)",
                            Reason = "Malformed <book> element"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(outerXml))
                    {
                        continue;
                    }

                    Book book;
                    try
                    {
                        var element = XElement.Parse(outerXml);
                        book = ParseBookElement(element);
                    }
                    catch (Exception)
                    {
                        result.InvalidBooks.Add(new InvalidBookEntry
                        {
                            Title = "(unknown)",
                            Reason = "Failed to parse <book> element"
                        });
                        continue;
                    }

                    if (!IsValidYear(book.Year, out var yearError))
                    {
                        result.InvalidBooks.Add(new InvalidBookEntry
                        {
                            Title = string.IsNullOrWhiteSpace(book.Title) ? "(unknown)" : book.Title,
                            Reason = yearError
                        });
                        continue;
                    }

                    if (!IsValidPublisher(book.Publisher, out var publisherError))
                    {
                        result.InvalidBooks.Add(new InvalidBookEntry
                        {
                            Title = string.IsNullOrWhiteSpace(book.Title) ? "(unknown)" : book.Title,
                            Reason = publisherError
                        });
                        continue;
                    }

                    result.ValidBooks.Add(book);
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (XmlException)
            {
                throw;
            }

            return result;
        }

        private static Book ParseBookElement(XElement bookElement)
        {
            var book = new Book
            {
                Title = bookElement.Element("title")?.Value?.Trim() ?? string.Empty,
                Author = bookElement.Element("author")?.Value?.Trim() ?? string.Empty,
                Genre = bookElement.Element("genre")?.Value?.Trim() ?? string.Empty,
                Publisher = bookElement.Element("publisher")?.Value?.Trim() ?? string.Empty
            };

            var yearText = bookElement.Element("year")?.Value?.Trim();
            book.Year = int.TryParse(yearText, out var y) ? y : -1;

            return book;
        }

        private static bool IsValidYear(int year, out string? error)
        {
            var currentYear = DateTime.UtcNow.Year;
            if (year <= 0)
            {
                error = "Year is missing or not a valid integer.";
                return false;
            }

            if (year > currentYear + 1) 
            {
                error = $"Year {year} is not in a reasonable range.";
                return false;
            }

            error = null;
            return true;
        }

        private static bool IsValidPublisher(string? publisher, out string? error)
        {
            if (string.IsNullOrWhiteSpace(publisher))
            {
                error = "Publisher is missing.";
                return false;
            }

            if (!Uri.TryCreate(publisher, UriKind.Absolute, out var uri))
            {
                error = "Publisher is not a valid absolute URI.";
                return false;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                error = "Publisher URI must use http or https scheme.";
                return false;
            }

            error = null;
            return true;
        }
    }
}