namespace UrbanNoodleTest.Helpers
{
    public class UtilServiceTest
    {
        [Fact]
        public void NormalizeText_ShouldReturnNormalizedString()
        {

            string input = "Café";
            string expected = "cafe";

            string result = UrbanNoodle.Utils.UtilService.NormalizeText(input);

            Assert.Equal(expected, result);
        }
    }
}
