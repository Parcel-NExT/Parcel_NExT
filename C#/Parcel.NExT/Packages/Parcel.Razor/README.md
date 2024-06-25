# Parcel.Razor

Status: Alpha
Remark: The usage of this library makes assembly not AOT-compliant as it depends heavily on reflection and/or compilation services.

Expose .Net Razor engine in a friendly generic format. This is a very thin wrapper around Net Core RazorEngine.

When using RazorEngine, one needs to reference RazorEngine and use something like the following:

```C#
using RazorEngine;
using RazorEngine.Templating;

Engine.Razor.RunCompile(template, templateKey, modelType, model);
```

When using this library, one needs to (in Pure) use the following setup - access the exposed model/dictionary using `@Model` or `@ViewBag`:

```C#
Import(Razor)
public class MyModel
{
    public string Name { get; set; }
    public double Age { get; set; }
}
MyModel model = new()
{
    Name = "James", 
    Age = 15
};
string result = Razor.Main.CompileTemplate(model, """
    Hello @Model.Name, you age is @Model.Age.
    """, Razor.TemplateFormat.Text);
```

```C#
Dictionary<string, object> values = new()
{
    { "TotalStudents", 15 },
};
string result = Razor.Main.CompileTemplate(values, """
    Total students: @ViewBag.TotalStudents
    """, Razor.TemplateFormat.Text);
Assert.Equal($"Total students: {values["TotalStudents"]}", result);
```

Tip: Use `@Raw()` or `<text></text>` to switch context in HTML format.

Notice due to limits of RazorEngine, one cannot use record types.

## TODO

- [ ] General package enhancement
- [ ] Update README and formats to conform to Parcel instead of Pure (2).

## Known Issue

Due to limits related to how complied in-memory types from Roslyn cannot be accessed inside Razor, we cannot use generic version of the CompileTemplate function inside Pure. This is a serious issue. You will experience error like (in Pure):

```
Errors while compiling a Template.
More details about the error:
 - error: (9, 112) Preprocessor directives must appear as the first non-whitespace character on a line
	 - error: (10, 4) Syntax error, '>' expected
	 - error: (9, 102) The type or namespace name 'Submission' could not be found (are you missing a using directive or an assembly reference?)
```

Current workaround is to use @ViewBag with the dictionary version of the function instead.