using System.Collections.Generic;

public class Pizza
{
    #region Enums

    private enum Size
    {
        Personal,
        Medium,
        Large
    }

    private enum Crust
    {
        HandTossed,
        Stuffed,
        Thin
    }

    private enum Sauce
    {
        Marinara,
        White,
        Barbeque,
        HotSauce
    }

    private enum Toppings
    {
        Pepperoni,
        Sausage,
        Meatball,
        Ham,
        Bacon,
        Chicken,
        Beef,
        Pork,
        Mushrooms,
        Spinach,
        Onion,
        Olives,
        BellPepper,
        Pineapple,
        Jalapeno
    }

    #endregion Enums

    private Size _size;
    private Crust _crust;
    private Sauce _sauce;
    private List<Toppings> _toppings;

    public Pizza()
    {
        //TODO Generate pizza
    }
}
