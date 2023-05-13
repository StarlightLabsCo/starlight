public static class ItemInstanceFactory
{
    public enum Items
    {
        CopperOre,
        Sword
    }

    public static Item Create(Items id)
    {
        switch (id)
        {
            case Items.CopperOre:
                return new CopperOre();
            case Items.Sword:
                return new Sword();
            default:
                return null;
        }
    }
}