using Xunit;

namespace AgritecDocFxPlugins.Tests;

public class QuestionNoteTest
{
    [Fact]
    public void NormalTextQuestion()
    {
        var content = @"::: question
This is a test
::: question-end";
        var expected = "<div class=\"NOTE\"><h5>QUESTION</h5><p>This is a test</p></div>";

        TestUtility.VerifyMarkup(content, expected);
    }

    [Fact]
    public void QuestionWithCode()
    {
        var content = @"::: question
This is a test
```
Also a test
```
::: question-end";
        var expected = "<div class=\"NOTE\"><h5>QUESTION</h5><p>This is a test</p><pre><code>Also a test</code></pre></div>";

        TestUtility.VerifyMarkup(content, expected);
    }


    [Fact]
    public void QuestionNoEnding()
    {
        var content = @"::: question
This is a test
";
        var expected = "<div class=\"NOTE\"><h5>QUESTION</h5><p>This is a test</p></div>";

        TestUtility.VerifyMarkup(content, expected, new[] { "invalid-question" });
    }


    [Fact]
    public void QuestionSameLine()
    {
        var content = @"::: question This is a test ::: question-end";
        var expected = "<div class=\"NOTE\"><h5>QUESTION</h5></div>";

        TestUtility.VerifyMarkup(content, expected, new[] { "invalid-question" });
    }
}
