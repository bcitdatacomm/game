# Coding Style

## Bracing
* {} should **always** be on the next line and are **not** optional, except for reason below.
    ```C#
    // Good
    if (var)
    {
        DoSomething();
    }
    else
    {
        DoSomethingElse();
    }

    // Bad
    if (thisThing && thatThing) {
        DoSomething();
    } else
        DoSomethingElse();

    // Single line functions can be on the same line

    // This is okay
    public class Foo
    {
        int bar;

        public int Bar
        {
            get { return bar; }
            set { bar = value; }
        }
    }
    ```

* Case statements **should** be indented from the switch statement, and **everything** inside the case statement should be indented from the case statement.
    ```C#
    switch (test)
    {
        case 0:
            DoSomething();
            break;
        case 1:
            DoSomethingElse();
            break;
        default:
            DoDefault();
            break;
    }
    ```

## Spacing
* Tabs should be **tabs** and **not** spaces.

* There should be **1** space after the , when listing parameters
    ```C#
    // Good
    Console.In.Read(var, 0, 1, 2, 3);

    // Bad
    Console.In.Read(var,0,1,2,3);

    // Same with arrays

    // Good
    x = dataAray[index];

    // Bad
    x = dataAray[ index ];
    ```

* There should **not** be a space between the function name and the parameters
    ```C#
    // Good
    Create()

    // Bad
    Create ()
    ```

## Naming Conventions
* Use **PascalCasing** for class names, public method names, and public variables.
    ```C#
    public class ClientActivity
    {
        public void ClearStats()
        {
            // Do Something
        }
    }
    ```

* Use **camelCasing** for private functions, arguments, and local variables.
    ```C#
    public class UserLog
    {
        public void Add(LogEvent logEvent)
        {
            int itemCount = logEvent.Items.Count;
        }
    }
    ```

* Do **not** use type identification prefixes.
    ```C#
    // Good
    int counter;
    string name;

    // Bad
    int iCounter;
    string strName;
    ```

* Do **not** use underscores in mutable variable names.
    ```C#
    // Bad
    int number_of_cats = 100;
    ```

* Constants should be in ALL_CAPS_WITH_UNDERSCORES
    ```C#
    // Good
    public static const string SHIPPING_TYPE = "normal";

    // Bad
    public static const string shipping_type = "fast";
    public static const string SHIPPINGTYPE = "snail";
    ```

* **Avoid** using abbreviations. Exceptions to this rule are commonly used names, such as **ID, XML, TCP, URI**. In this case use **camelCasing** for abbreviations.
    ```C#
    // Good
    HtmlHelper htmlHelper;
    FtpTransfer ftpTransfer;
    UIControl uiControl;

    // Bad
    HtmlHelper HTMLHelper;
    FTPTransfer FtpTransfer;
    ```

## Member variables
* **Always** use C# getters and setter with public members
	``` C#
	// Good
	public int Health { get; set; }

	// Bad
	public int Health;
	```

* **Always** use the 'this' keyword when accessing member variables to improve readability
	``` C#
	public Car
	{
		private int model;

		public Car()
		{
			// Good
			this.model = 0;
			
			// Bad
			model = 0;
		}
	}
	```
