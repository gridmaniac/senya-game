using UnityEngine.Video;

public static class GameStatic {
    public static int id = 1;
    public static int state = 1;

    public static int[] typesCountTable = { 0, 0, 0, 10, 20, 1500, 3500, 7500 };
    public static float pawChance = 0.2f;

    public static VideoClip video;

    public static string quiz1 = "Что делать, если потерялся в магазине?";
    public static string[] quiz1variants = { "Подойти к продавцу, кассиру",
    "Стоять у выхода", "Кричать «Ау»", "Плакать" };

    public static string quiz2 = "Что важно согласовать перед тем, как пойти на городской праздник, в парк или супермаркет?";
    public static string[] quiz2variants = { "Место встречи",
    "Время на часах", "Сумму расходов", "Пароль" };

    public static string quiz3 = "Какое место лучше выбрать для встречи, если потерялся в городе?";
    public static string[] quiz3variants = { "Аптеку",
    "Кинотеатр", "Остановку", "Пешеходный переход" };

    public static int pawsCount = 0;
    public static int scoreCount = 0;
}
