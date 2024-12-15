using System;

namespace KoftaAndKonafa.Enums
{
    public static class GameEnums
    {
      
        public enum StationType
        {
            ResourceStation,
            HeatStation,
            ChoppingStation,
            AssembleStation,
            CookStation,
            DeliveryStation,
            FireStation,
            DonationStation,
            None
        }
        
        public enum IngredientState
        {
            Default,
            Chopped,
            Heated,
            Cooked,
            NotEatable
            
        }

        public enum Ingredient
        {
            Potato,
            Onion,
            Chicken,
            Bun,
            BurgerPatty,
            ChickenPatty,
            Cheese,
            Meat,
            Tomato,
            Cucumber,
            Pancake,
            Syrup,
            Egg
        }

        public enum PlacementPoint
        {
           Left,
           MiddleLeft,
           MiddleRight,
           Right
        }
        
        public enum Item
        {
            Ingredient,
            Meal,
            UncookedMeal,
            FireExtinguisher,
            Empty
        }

      
    }
}