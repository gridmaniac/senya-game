using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Grid : MonoBehaviour {

    /// <summary>
    /// Ширина сетки
    /// </summary>
    private const int w = 9;

    /// <summary>
    /// Высота сетки
    /// </summary>
    private const int h = 9;

    /// <summary>
    /// Признак пустого шара
    /// </summary>
    private const int EMPTY = -1;

    /// <summary>
    /// Признак пустой ячейки
    /// </summary>
    private const int PASS = -2;

    /// <summary>
    /// Признак занятой ячейки
    /// </summary>
    private const int IMPASS = -1;

    /// <summary>
    /// Коэффициенты для наложения сетки
    /// </summary>
    private const float o = 0.18f;
    private const float kx = 0.19f;
    private const float ky = -0.17f;

    /// <summary>
    /// Матрица для расчета оптимального пути
    /// </summary>
    private int[,] gSpace = new int[w, h];

    /// <summary>
    /// Матрица с расположением шаров (цвета)
    /// </summary>
    private int[,] gLayout = new int[w, h];

    /// <summary>
    /// Дополнительная матрица для расчета пути
    /// </summary>
    private int[,] grid;

    /// <summary>
    /// Результирующая длина оптимального пути
    /// </summary>
    private int len;

    /// <summary>
    /// Результирующие точки оптимального пути
    /// </summary>
    private int[] px = new int[w * h], 
        py = new int[w * h];

    /// <summary>
    /// Ячейки
    /// </summary>
    private List<Cell> cells = new List<Cell>();

    /// <summary>
    /// Выбранная ячейка
    /// </summary>
    private Cell selectedCell;

    /// <summary>
    /// Маршрут для перемещения шара
    /// </summary>
    private List<Cell> route = new List<Cell>();

    /// <summary>
    /// Начальная позиция спауна шаров
    /// </summary>
    private Vector3 spawnPosition = new Vector3(20, 20, 0);

    /// <summary>
    /// Смещение относительно ячейки спавна
    /// </summary>
    private float cellSpawnOffset = 20.0f;

    /// <summary>
    /// Стек шаров для спавна
    /// </summary>
    private List<Ball> ballsToSpawn = new List<Ball>();

    /// <summary>
    /// Следующие цвета
    /// </summary>
    private int[] nextColors;

    /// <summary>
    /// Количество следующих шаров
    /// </summary>
    private int nextCount;

    /// <summary>
    /// Активный шар
    /// </summary>
    private Ball activeBall;

    /// <summary>
    /// Направление анимации шара при уничтожении
    /// </summary>
    private int dieDirection = 1;

    /// <summary>
    /// Задержки при уничтожении шаров
    /// </summary>
    private float deathDelay = 0;

    /// <summary>
    /// Признак блокировки сетки
    /// </summary>
    private bool isBlocked = false;

    /// <summary>
    /// Количество очков
    /// </summary>
    private int score = 0;

    /// <summary>
    /// Аккумулятор очков
    /// </summary>
    private int scoreMultiplier = 0;

    private bool hasActivePaw = false;
    private int comboCount = 0;
    private int quizCounter = 0;
    private bool isRestQuiz = false;
    
    /// <summary>
    /// Префаб ячейки
    /// </summary>
    [Header("Префаб ячейки")]
    public GameObject prefabCell;

    /// <summary>
    /// Префаб шара
    /// </summary>
    [Header("Префаб шара")]
    public GameObject prefabBall;

    /// <summary>
    /// Контейнер для ячеек
    /// </summary>
    [Header("Контецнер для ячеек")]
    public Transform cellContainer;

    /// <summary>
    /// Контейнер для шаров
    /// </summary>
    [Header("Контейнер для шаров")]
    public Transform balls;

    /// <summary>
    /// Ссылки на следующие шары
    /// </summary>
    [Header("Ссылки на следующие шары")]
    public NextBall[] nextBalls;

    /// <summary>
    /// Объект очков
    /// </summary>
    [Header("Объект очков")]
    public Score scoreLamps;

    /// <summary>
    /// Звук при влете шара
    /// </summary>
    [Header("Звук при влете шара")]
    public AudioSource dropSound;

    /// <summary>
    /// Звук прыжка шаров
    /// </summary>
    [Header("Звук прыжка шаров")]
    public AudioSource JumpSound;

    /// <summary>
    /// Звук найденной последовательности
    /// </summary>
    [Header("Звук найденной последовательности")]
    public AudioSource ClusterSound;

    /// <summary>
    /// Звук при показе следующих шаров
    /// </summary>
    [Header("Звук при показе следующих шаров")]
    public AudioSource NextSound;

    /// <summary>
    /// Инициализация сетки
    /// </summary>
    void initGrid()
    {
        //Получаем компонент рендера сетки
        var renderer = GetComponent<SpriteRenderer>();

        //Вычисляем смещения для ячеек
        var stepX = (renderer.bounds.size.x - o * 2.0f) / w;
        var stepY = (renderer.bounds.size.y - o * 2.0f) / h;

        //Вычисляем отправную координату для ячеек
        var origin = this.transform.position 
            + new Vector3(kx, ky, 0);

        //Начинаем создание ячеек
        for (var j = 0; j < h; j++)
        {
            for (var i = 0; i < w; i++)
            {
                //Создаем игровой объект ячейки и размещаем его
                var cellObject = Instantiate(prefabCell, 
                    new Vector3(origin.x + i * stepX + stepX * 0.5f, 
                    origin.y - j * stepY - stepY * 0.5f, 0), 
                    Quaternion.identity, this.cellContainer);

                //Получаем компонент ячейки
                var cell = cellObject.GetComponent<Cell>();

                //Задаем координаты
                cell.SetCoord(new Vector2(i, j));

                //Добавляем в пул
                this.cells.Add(cell);
            }
        }
    }

    //Инициализация компонента
    void Start () {
        Application.targetFrameRate = 60;

        //Заполняем матрицы сетки начальными значениями
        for (var i = 0; i < h; i++)
        {
            for (var j = 0; j < w; j++)
            {
                this.gSpace[j, i] = PASS;
                this.gLayout[j, i] = EMPTY;
            }
        }

        //Инициализируем сетку
        this.initGrid();

        this.nextCount = nextBalls.Length;
        this.nextColors = new int[this.nextCount];

        StartCoroutine(GenerateBallsDelayed());
    }
	
	//Вызывается каждый кадр
	void Update () {
        this.FollowRoute();
        this.SpawnBalls();
    }

    private void SpawnBalls()
    {
        //Если в стеке спавна остались шары и режим спавна активен
        if (this.ballsToSpawn.Count > 0 && !this.isBlocked)
        {
            //Если первый шар в стеке не был инициализирован
            if (!this.ballsToSpawn[0].IsInitialized)
            {
                //Получить массив свободных ячеек для спавна
                var cells = this.GetFreeCells();

                //Если найдены свободные ячейки
                if (cells.Count > 0)
                {
                    //Выбираем случайную ячейку
                    var c = Random.Range(0, cells.Count);

                    //Забираем ее позицию
                    var pos = cells[c].pos;

                    //Ставим шар над выбранной ячейкой
                    this.ballsToSpawn[0].pos =
                        new Vector3(pos.x, pos.y + this.cellSpawnOffset, 0);

                    //Инициализируем шар (задаем конечную позицию и цвет)
                    this.ballsToSpawn[0].Init(pos,
                        this.nextColors[this.nextCount - this.ballsToSpawn.Count]);

                    //Помещаем шар в ячейку на сетке
                    this.OccupyCell(cells[c], this.ballsToSpawn[0]);
                }
            }

            var ball = this.ballsToSpawn[0];
            ball.Z(3);
            ball.Move(Time.deltaTime, Time.deltaTime);

            if (!ball.IsDistant)
            {
                if (this.ballsToSpawn.Count == 1)
                {
                    if (this.ClusterCheck())
                    {
                        this.ClusterSound.Play();
                        this.comboCount++;
                    }
                    else
                    {
                        this.comboCount = 0;
                    }
                        
                    this.PredictBalls(this.nextCount);
                    this.ShowNextBalls();
                }

                if (this.comboCount > 1)
                {
                    var gameController = FindObjectOfType<GameController>();
                    switch (this.comboCount)
                    {
                        case 2:
                            gameController.notifier.NotifyCombo2();
                            this.score += 100;
                            break;
                        case 3:
                            gameController.notifier.NotifyCombo3();
                            this.score += 200;
                            break;
                        case 4:
                            gameController.notifier.NotifyCombo4();
                            this.score += 300;
                            break;
                        case 5:
                            gameController.notifier.NotifyCombo5();
                            this.score += 400;
                            break;
                        case 6:
                            gameController.notifier.NotifyCombo6();
                            this.score += 500;
                            break;
                    }

                    gameController.score.SetScore(this.score);
                }

                this.ballsToSpawn[0].AnimDrop();
                this.ballsToSpawn.RemoveAt(0);
                this.dropSound.time = 0.002f;
                this.dropSound.Play();
                ball.Z(2);
            }
        }
    }

    /// <summary>
    /// Циклическая функция прохождения по маршруту
    /// </summary>
    private void FollowRoute()
    {
        //Выполняем проход по маршруту
        if (this.route.Count > 0)
        {
            this.activeBall.SetTarget(route[0].pos);
            this.activeBall.Move(Time.deltaTime * 24.0f, Time.deltaTime * 14.0f);

            if (!this.activeBall.IsDistant)
            {
                this.route.RemoveAt(0);
                this.JumpSound.Play();
            }
        }
        else if (this.activeBall != null)
        {
            this.activeBall.AnimIdle();
            this.activeBall = null;
            this.selectedCell = null;
            
            if (!this.ClusterCheck())
            {
                this.HideNextBalls();
                this.GenerateBalls(this.nextCount);
                this.NextSound.Play();
                this.comboCount = 0;
            }
            else
            {
                this.ClusterSound.Play();
                this.comboCount++;
            }

            if (this.comboCount > 1)
            {
                var gameController = FindObjectOfType<GameController>();
                switch (this.comboCount)
                {
                    case 2:
                        gameController.notifier.NotifyCombo2();
                        this.score += 100;
                        break;
                    case 3:
                        gameController.notifier.NotifyCombo3();
                        this.score += 200;
                        break;
                    case 4:
                        gameController.notifier.NotifyCombo4();
                        this.score += 300;
                        break;
                    case 5:
                        gameController.notifier.NotifyCombo5();
                        this.score += 400;
                        break;
                    case 6:
                        gameController.notifier.NotifyCombo6();
                        this.score += 500;
                        break;
                }

                gameController.score.SetScore(this.score);
            }
        }
    }

    /// <summary>
    /// Сгенерировать шары
    /// </summary>
    /// <param name="count">Количество шаров</param>
    private void GenerateBalls(int count)
    {
        var freeCellsCount = this.GetFreeCells().Count;
        if (freeCellsCount > 2)
        {
            this.ballsToSpawn.Clear();

            for (var i = 0; i < count; i++)
            {
                var ballObject = Instantiate(prefabBall, this.spawnPosition, Quaternion.identity, balls);
                var ball = ballObject.GetComponentInChildren<Ball>();
                ballsToSpawn.Add(ball);
            }
        }
        else
        {
            FindObjectOfType<GameController>().GameOver();
        }
    }

    private IEnumerator GenerateBallsDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        this.GenerateBalls(this.nextCount);
    }

    private void PredictBalls(int count)
    {
        bool isPawTried = false;
        for (var i = 0; i < count; i++)
        {
            bool isPaw = false;
            if (this.scoreLamps.Range > 4 
                && !isPawTried 
                && GameStatic.state != 2 
                && !this.hasActivePaw)
            {
                float pawChance = GameStatic.pawChance;
                if (Random.value > 1.0f - pawChance)
                {
                    this.nextColors[i] = 8;
                    isPaw = true;
                    this.hasActivePaw = true;
                }
                isPawTried = true;
            }

            if (!isPaw)
            {
                //Выбираем случайный цвет из диапазона
                this.nextColors[i] = Random.Range(0, this.scoreLamps.Range);
            }
        }

        if (this.scoreLamps.Range > 3)
        {

        }
    }

    /// <summary>
    /// Установить шар в ячейку и занять место
    /// </summary>
    /// <param name="cell">Ячейка</param>
    /// <param name="ball">Шар</param>
    private void OccupyCell(Cell cell, Ball ball)
    {
        //Получаем координаты ячейки
        int x = (int)cell.GetCoord.x,
            y = (int)cell.GetCoord.y;

        //Получаем цвет шара
        int color = ball.Color;

        //Помещаем шар в ячейку
        cell.Ball = ball;

        //Обновляем матрицы
        gSpace[x, y] = IMPASS;
        gLayout[x, y] = color;
    }

    /// <summary>
    /// Освободить ячейку
    /// </summary>
    /// <param name="cell">Ячейка</param>
    /// <param name="ball">Шар</param>
    private void FreeCell(Cell cell)
    {
        //Получаем координаты ячейки
        int x = (int)cell.GetCoord.x,
            y = (int)cell.GetCoord.y;

        //Убираем шар из ячейки
        this.selectedCell.Ball = null;

        //Обновляем матрицы
        gSpace[x, y] = PASS;
        gLayout[x, y] = EMPTY;
    }

    /// <summary>
    /// Получить набор пустых ячеек
    /// </summary>
    /// <returns>Список пустых ячеек</returns>
    private List<Cell> GetFreeCells()
    {
        List<Cell> result = new List<Cell>();
        for (var i = 0; i < h; i++)
            for (var j = 0; j < w; j++)
                if (gSpace[j, i] == PASS)
                    result.Add(this.GetCellByCoord(new Vector2(j, i)));

        return result;
    }

    /// <summary>
    /// Получить набор непустых ячеек
    /// </summary>
    /// <returns>Список непустых ячеек</returns>
    private List<Cell> GetOccupiedCells()
    {
        List<Cell> result = new List<Cell>();
        for (var i = 0; i < h; i++)
            for (var j = 0; j < w; j++)
                if (gSpace[j, i] == IMPASS)
                    result.Add(this.GetCellByCoord(new Vector2(j, i)));

        return result;
    }

    /// <summary>
    /// Проверить возможность выполнить ход
    /// </summary>
    /// <param name="cell">Целевая ячейка</param>
    public void TryTurn(Cell cell)
    {
        //Если игрок выбрал начальную ячейку
        if (this.selectedCell != null 
            && this.activeBall == null)
        {
            var from = selectedCell.GetCoord;
            var to = cell.GetCoord;

            //Ищем оптимальный маршрут
            if (this.Lee((int)from.x, (int)from.y, (int)to.x, (int)to.y))
            {
                //Если маршрут найден
                int k = 0;
                do
                { 
                    k++;
                    var c = this.GetCellByCoord(new Vector2(px[k], py[k]));
                    this.route.Add(c);
                }
                while (k < len);

                this.OccupyCell(
                    this.GetCellByCoord(new Vector2(to.x, to.y)),
                    selectedCell.Ball);

                this.activeBall = this.selectedCell.Ball;
                this.activeBall.AnimBounce();

                this.JumpSound.Play();

                this.FreeCell(
                    this.GetCellByCoord(new Vector2(from.x, from.y)));

                this.quizCounter++;

                if (this.quizCounter > 10 && !this.isRestQuiz)
                {
                    this.isRestQuiz = true;
                    this.quizCounter = 0;
                    FindObjectOfType<GameController>().notifier.NotifyQuiz();
                }

                if (this.quizCounter > 15)
                {
                    this.quizCounter = 0;
                    if (Random.value > 0.8f)
                    {
                        FindObjectOfType<GameController>().notifier.NotifyQuiz();
                    }
                }
            }
        }
    }

    private Cell GetCellByCoord(Vector2 coord)
    {
        foreach (var cell in cells)
        {
            if (cell.GetCoord == coord)
            {
                return cell;
            }
        }
        return null;
    }

    public void SelectCell(Cell cell)
    {
        this.selectedCell = cell;
    }

    public void UnselectCell()
    {
        this.selectedCell = null;
    }

    private void ShowNextBalls()
    {
        if (Random.Range(0, 2) == 0)
        {
            StartCoroutine(ShowNextBall(0.0f, 0));
            StartCoroutine(ShowNextBall(0.2f, 1));
            StartCoroutine(ShowNextBall(0.1f, 2));
        }
        else
        {
            StartCoroutine(ShowNextBall(0.0f, 0));
            StartCoroutine(ShowNextBall(0.1f, 1));
            StartCoroutine(ShowNextBall(0.2f, 2));
        }
    }

    public void HideNextBalls()
    {
        for (var i = 0; i < this.nextBalls.Length; i++)
        {
            this.nextBalls[i].Hide();
        }
    }

    IEnumerator ShowNextBall(float delay, int k)
    {
        yield return new WaitForSeconds(delay);
        this.nextBalls[k].Show(this.nextColors[k]);
    }

    private bool Lee(int ax, int ay, int bx, int by)
    {
        int[] dx = new int[] { 1, 0, -1, 0 };
        int[] dy = new int[] { 0, 1, 0, -1 };
        int d, x, y, k;
        bool stop;
        grid = (int[,])gSpace.Clone();

        stop = grid[bx, by] == IMPASS;
        if (stop) return false;

        d = 0;
        grid[ax, ay] = 0;

        do
        {
            stop = true;
            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x++)
                {
                    if (grid[x, y] == d)
                    {
                        for (k = 0; k < 4; k++)
                        {
                            int ix = x + dx[k], iy = y + dy[k];
                            if (ix >= 0 && ix < w && iy >= 0 && iy < h && grid[ix, iy] == PASS)
                            {
                                stop = false;
                                grid[ix, iy] = d + 1; 
                            }
                        }
                    }
                }
            }
            d++;
        }
        while (!stop && grid[bx, by] == PASS);

        stop = grid[bx, by] == PASS;
        if (stop) return false;

        len = grid[bx, by];
        x = bx;
        y = by;
        d = len;

        while (d > 0)
        {
            px[d] = x;
            py[d] = y;
            d--;
            for (k = 0; k < 4; k++)
            {
                int ix = x + dx[k], iy = y + dy[k];
                if (ix >= 0 && ix < w && iy >= 0 && iy < h && grid[ix, iy] == d)
                {
                    x = x + dx[k];
                    y = y + dy[k];
                    break;
                }
            }
        }

        px[0] = ax;
        py[0] = ay;

        return true;
    }

    /// <summary>
    /// Счетчик шаров одинакового цвета
    /// </summary>
    private int ballsCounter;

    /// <summary>
    /// Пул из последовательностей одинаковых шаров
    /// </summary>
    private List<List<Cell>> ballsPool;

    /// <summary>
    /// Последовательность одинаковых шаров
    /// </summary>
    private List<Cell> ballsStack;

    private void CheckSequence(int j, int i)
    {
        int color = this.gLayout[j, i];

        if (color != EMPTY)
        {
            if (this.ballsStack.Count == 0)
            {
                this.ballsStack.Add(this.GetCellByCoord(new Vector2(j, i)));
                this.ballsCounter = 1;
            }
            else
            {
                if (this.ballsStack[this.ballsCounter - 1].Ball.Color == color 
                    || this.ballsStack[this.ballsCounter - 1].Ball.Color + color == 12)
                {
                    this.ballsStack.Add(this.GetCellByCoord(new Vector2(j, i)));
                    this.ballsCounter++;
                }
                else
                {
                    this.FindCluster();
                    this.ballsStack.Add(this.GetCellByCoord(new Vector2(j, i)));
                    this.ballsCounter = 1;
                }
            }
        }
        else
        {
            this.FindCluster();
            this.ballsCounter = 0;
        }
    }

    private void FindCluster()
    {
        if (this.ballsStack.Count > 4)
        {
            this.scoreMultiplier = 0;
            List<Cell> temp = new List<Cell>();
            foreach (var cell in this.ballsStack)
            {
                if (this.scoreMultiplier > 4 || cell.Score == 2)
                    cell.Score = 5;
                else
                    cell.Score = 2;

                temp.Add(cell);
                this.scoreMultiplier++;
            }
            this.scoreMultiplier = 0;
            this.ballsPool.Add(temp);
        }

        this.ballsStack.Clear();
    }

    private bool ClusterCheck()
    {
        this.ballsPool = new List<List<Cell>>();
        this.ballsStack = new List<Cell>();

        for (var i = 0; i < h; i++)
        {
            this.ballsCounter = 0;
            for (var j = 0; j < w; j++)
            {
                this.CheckSequence(j, i);
            }

            this.FindCluster();
        }

        for (var j = 0; j < w; j++)
        {
            this.ballsCounter = 0;
            for (var i = 0; i < h; i++)
            {
                this.CheckSequence(j, i);
            }

            this.FindCluster();
        }

        for (int k = 4; k < w * 2 - 5; k++)
        {
            this.ballsCounter = 0;
            for (int j = 0; j <= k; j++)
            {
                int i = k - j;
                if (i < w && j < w)
                {
                    this.CheckSequence(j, i);
                }
            }

            this.FindCluster();
        }

        for (int k = w - 5; k >= -w + 5; k--)
        {
            this.ballsCounter = 0;
            for (int j = 0; j < w - k; j++)
            {
                int i = k + j;
                if (i < w && j < w && i >= 0 && j >= 0)
                {
                    this.CheckSequence(j, i);
                }
            }

            this.FindCluster();
        }

        foreach (var p in this.ballsPool)
        {
            foreach (var s in p)
            {
                this.isBlocked = true;
                int x = (int)s.GetCoord.x,
                    y = (int)s.GetCoord.y;

                int variant = 1;

                if (x < 2)
                {
                    if (y < 2)
                        variant = 1;
                    else if (y > 1 && y < 4)
                        variant = 2;
                    else
                        variant = 3;
                }
                else if (x > 7)
                {
                    if (y < 2)
                        variant = 4;
                    else if (y > 1 && y < 4)
                        variant = 5;
                    else
                        variant = 6;
                }
                else if (x > 1 && x < 8 & y < 2)
                {
                    if (this.dieDirection > 0)
                        variant = 1;
                    else
                        variant = 4;
                }
                else if (x > 1 && x < 8 && y > 1 && y < 4)
                {
                    if (this.dieDirection > 0)
                        variant = 2;
                    else
                        variant = 5;
                }
                else
                {
                    if (this.dieDirection > 0)
                        variant = 3;
                    else
                        variant = 6;
                }

                if (s.Ball != null && s.Ball.Color == 8)
                {
                    //Лапка
                    GameStatic.pawsCount++;
                    FindObjectOfType<GameController>().ShowNextPaw();
                    this.hasActivePaw = false;

                }

                StartCoroutine(KillBall(s.Ball, variant));
                StartCoroutine(ShowScore(s));
                this.score += s.Score;
                this.dieDirection *= -1;
                this.deathDelay += 0.2f;

                gSpace[x, y] = PASS;
                gLayout[x, y] = EMPTY;
                s.Ball = null;
            }

            StartCoroutine(Unlock());

            FindObjectOfType<Score>().SetScore(this.score);

            this.deathDelay = 0;
            this.dieDirection = 1;
        }

        return this.ballsPool.Count > 0;
    }

    private IEnumerator KillBall(Ball ball, int death)
    {
        yield return new WaitForSeconds(this.deathDelay);
        if (ball != null)
            ball.Die(death);
    }

    private IEnumerator ShowScore(Cell cell)
    {
        yield return new WaitForSeconds(this.deathDelay);
        cell.ShowScore();
    }

    private IEnumerator Unlock()
    {
        yield return new WaitForSeconds(this.deathDelay);
        this.isBlocked = false;
    }

    public void Kill9Balls()
    {
        var cells = this.GetOccupiedCells();
        if (cells.Count <= 9)
        {
            int noLastOne = cells.Count;
            foreach (var cell in cells)
            {
                if (noLastOne > 1)
                {
                    int x = (int)cell.GetCoord.x,
                        y = (int)cell.GetCoord.y;

                    int variant = 1;

                    if (x < 2)
                    {
                        if (y < 2)
                            variant = 1;
                        else if (y > 1 && y < 4)
                            variant = 2;
                        else
                            variant = 3;
                    }
                    else if (x > 7)
                    {
                        if (y < 2)
                            variant = 4;
                        else if (y > 1 && y < 4)
                            variant = 5;
                        else
                            variant = 6;
                    }
                    else if (x > 1 && x < 8 & y < 2)
                    {
                        if (this.dieDirection > 0)
                            variant = 1;
                        else
                            variant = 4;
                    }
                    else if (x > 1 && x < 8 && y > 1 && y < 4)
                    {
                        if (this.dieDirection > 0)
                            variant = 2;
                        else
                            variant = 5;
                    }
                    else
                    {
                        if (this.dieDirection > 0)
                            variant = 3;
                        else
                            variant = 6;
                    }

                    this.dieDirection *= -1;

                    StartCoroutine(KillBall(cell.Ball, variant));
                    this.dieDirection *= -1;

                    gSpace[x, y] = PASS;
                    gLayout[x, y] = EMPTY;
                    cell.Ball = null;
                }
                noLastOne--;
            }
        }
        else
        {
            for (var i = 0; i < 9; i++)
            {
                cells = this.GetOccupiedCells();
                int k = Random.Range(0, cells.Count);
                var cell = cells[k];

                int x = (int)cell.GetCoord.x,
                    y = (int)cell.GetCoord.y;

                int variant = 1;

                if (x < 2)
                {
                    if (y < 2)
                        variant = 1;
                    else if (y > 1 && y < 4)
                        variant = 2;
                    else
                        variant = 3;
                }
                else if (x > 7)
                {
                    if (y < 2)
                        variant = 4;
                    else if (y > 1 && y < 4)
                        variant = 5;
                    else
                        variant = 6;
                }
                else if (x > 1 && x < 8 & y < 2)
                {
                    if (this.dieDirection > 0)
                        variant = 1;
                    else
                        variant = 4;
                }
                else if (x > 1 && x < 8 && y > 1 && y < 4)
                {
                    if (this.dieDirection > 0)
                        variant = 2;
                    else
                        variant = 5;
                }
                else
                {
                    if (this.dieDirection > 0)
                        variant = 3;
                    else
                        variant = 6;
                }

                this.dieDirection *= -1;

                StartCoroutine(KillBall(cell.Ball, variant));
                this.dieDirection *= -1;

                gSpace[x, y] = PASS;
                gLayout[x, y] = EMPTY;
                cell.Ball = null;
            }
        }
        this.dieDirection = 1;
    }
}
