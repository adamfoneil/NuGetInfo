using NuGetInfo.CLI.Static;

namespace Testing
{
    [TestClass]
    public class DateParsing
    {
        [TestMethod]
        [DataRow("downloads-2023-02-05.json", 2023, 2, 5)]
        public void FilenameShouldExtractDate(string path, int year, int month, int day)
        {
            var date = Parse.DateFromPath(path);
            Assert.IsTrue(date.Equals(new DateOnly(year, month, day)));
        }
    }
}