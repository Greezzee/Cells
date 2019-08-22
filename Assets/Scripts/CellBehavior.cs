using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gen
{
    public int ID, info, adInfo;
}
/* Свойства:
 * 0 - здоровье (+10 максимального здоровья за элемент) +2g, +1b
 * 1 - скорость (+0.5 хода за элемент) +2r, +1b
 * 2 - бонус фотосинтеза (+10% эффективности фотосинтеза, -5% эффективности хищничества и хемосинтеза) +3g
 * 3 - бонус хемосинтеза (+10% эффективности хемосинтеза, +2 ёмкости запаса минералов, -5% эффективности фотосинтеза и хищничества) +3b
 * 4 - бонус мусорничества (+20% эффективности поедания трупов за элемент, -5% эффективности фотосинтеза и хемосинтеза) +2r, +1g
 * 5 - бонус хищничества (+10% эффективности хищничества, +1 к силе, -5% эффективности фотосинтеза и хемосинтеза) +3r
 * 
 * 
 * 
 * 
 * 
 * Поведение: (64 элемента)
 * 0 - простой
 * 1 - фотосинтез
 * 2 - двигаться
 *     0 - вверх
 *     1 - вправо
 *     2 - вниз
 *     3 - влево
 * 3 - поворот
 *     0 - по часовой
 *     1 - против часовой
 * 4 - переработать минералы
 * 5 - нападение, прыгнуть при успехе
 *     0 - вверх
 *     1 - вправо
 *     2 - вниз
 *     3 - влево 
 *     
 *     
 * 6 - безусловный прыжок (новое положение в adInfo)
 * 7 - прыжок, если рядом есть клетка
 * 8 - прыжок, если рядом есть труп
 * 9 - прыжок, если с определённой стороны есть клетка
 * 10 - прыжок, если с определённой стороны есть труп
 * 11 - прыгнуть, если ниже данного уровня
 * 12 - прыгнуть, если выше данного уровня
 * 
 * 13 - прыгнуть, если с определённой стороны есть хотя бы одна чужая клетка
 * 14 - прыгнуть, если с определённой стороны есть хотя бы одна родственная клетка
 * 15 - прыгнуть, если рядом есть чужая клетка 
 * 16 - прыгнуть, если рядом есть родственная клетка
 * 
 * 17 - прыгнуть, если число в регистре больше определённого
 * 18 - прыгнуть, если число в регистре меньше определённого
 * 19 - прыгнуть, если число в регистре равно определённому
 * 
 * 20 - считать число с регистра определённой клетки
 * 21 - записать число в регистр определённой клетки
 * 
 * 22 - установить регистр в данное положение
 * 
 * 23 - поделиться досрочно
 * 
 * 24 - прыжок, если здоровье больше определённого значения
 * 25 - прыжок, если здоровье меньше определённого значения
 * 26 - выбросить здоровье
 * 27 - передать здоровье клетке с определённой стороны
*/
public class CellBehavior : MonoBehaviour
{
    public bool isEated = false;
    public int register = 0; //Значения от 0 до 7

    static int colorType = 0;

    private int behCount = 28, propCount = 6;
    private int behDNAsize = 64, propDNAsize = 64;
    public GameObject obj;
    private enum directions {up, right, down, left};
    private Vector2Int boardSize = new Vector2Int(75, 49);

    [SerializeField]
    private float MaxHealth = 100;

    public float Health;
    public int DebugID;
    public Gen[] behDNA;
    public int[] propDNA;

    private int step;
    private float maxCycle = 5;
    private int rotation = 0;

    private float mineralBuffer = 0f, maxMinerals = 10f;

    private int stepsToCorpseDisapp = 300, CorpseStep;

    [SerializeField]
    private float photoMultiplier = 1f, chemoMultiplier = 1f, predoMultiplier = 1f, trashMultiplier = 1f;
    public int predatorPower;


    private Color behColor, propColor;
    public Color familyColor;

    public GameObject world;
    private WorldInfo wi;

    void Start()
    {
        wi = world.GetComponent<WorldInfo>();

        name = "Cell";
        MaxHealth = 100;
        maxCycle = 5;
        rotation = 0;
        mineralBuffer = 0f;
        maxMinerals = 10;
        stepsToCorpseDisapp = 300;
        photoMultiplier = 1f;
        chemoMultiplier = 1f;
        trashMultiplier = 1f;
        predoMultiplier = 1f;
        predatorPower = 0;
        step = 0;
        isEated = false;
        if (behDNA.Length == 0)
        {
            behDNA = new Gen[behDNAsize];
            for (int i = 0; i < behDNA.Length; i++)
            {
                Gen test = new Gen();
                test.ID = DebugID;
                test.info = 1;
                test.adInfo = 0;
                behDNA[i] = test;
            }
        }
        if (propDNA.Length == 0)
        {
            propDNA = new int[propDNAsize];
            if (DebugID == 1) for (int i = 0; i < propDNA.Length; i++) propDNA[i] = 2;
            else if (DebugID == 4) for (int i = 0; i < propDNA.Length; i++) propDNA[i] = 3;
            else for (int i = 0; i < propDNA.Length; i++) propDNA[i] = Random.Range(0, propCount);
        }


        int red = 0, green = 0, blue = 0;
        for (int i = 0; i < propDNA.Length; i++) switch (propDNA[i])
            {
                case 0:
                    MaxHealth += 10;
                    green += 2;
                    blue += 1;
                    break;
                case 1:
                    maxCycle += 0.5f;
                    red += 2;
                    blue += 1;
                    break;
                case 2:
                    photoMultiplier += 0.15f;
                    chemoMultiplier -= 0.05f;
                    predoMultiplier -= 0.05f;
                    trashMultiplier -= 0.05f;
                    green += 3;
                    break;
                case 3:
                    chemoMultiplier += 0.15f;
                    photoMultiplier -= 0.05f;
                    predoMultiplier -= 0.05f;
                    trashMultiplier -= 0.05f;
                    maxMinerals += 2f;
                    blue += 3;
                    break;
                case 4:
                    trashMultiplier += 0.2f;
                    chemoMultiplier -= 0.05f;
                    predoMultiplier -= 0.05f;
                    photoMultiplier -= 0.05f;
                    red += 2;
                    green += 3;
                    break;
                case 5:
                    predoMultiplier += 0.1f;
                    chemoMultiplier -= 0.05f;
                    photoMultiplier -= 0.05f;
                    trashMultiplier -= 0.05f;
                    predatorPower++;
                    red += 3;
                    //WIP
                    break;
            }
        if (predoMultiplier < 0) predoMultiplier = 0;
        if (chemoMultiplier < 0) chemoMultiplier = 0;
        if (photoMultiplier < 0) photoMultiplier = 0;
        if (trashMultiplier < 0) trashMultiplier = 0;
        propColor = new Color((float)red / 192f, (float)green / 192f, (float)blue / 192f);

        red = 0; green = 0; blue = 0;
        int sum = 0;
        for (int i = 0; i < behDNA.Length; i++)
        {
            switch (behDNA[i].ID)
            {
                case 1:
                    green++;
                    sum++;
                    break;
                case 4:
                    blue++;
                    sum++;
                    break;
                case 5:
                    red++;
                    sum++;
                    break;
            }
        }
        behColor = new Color((float)red / (float)sum, (float)green / (float)sum, (float)blue / (float)sum);
        if (colorType == 1) GetComponent<SpriteRenderer>().color = propColor;
        if (colorType == 2) GetComponent<SpriteRenderer>().color = behColor;
        if (colorType == 3) GetComponent<SpriteRenderer>().color = familyColor;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GetComponent<SpriteRenderer>().color = propColor;
            colorType = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            colorType = 2;
            GetComponent<SpriteRenderer>().color = behColor;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            colorType = 3;
            GetComponent<SpriteRenderer>().color = familyColor;
        }
    }
    
    void FixedUpdate()
    {
        if (isEated)
        {
            Destroy(gameObject);
            return;
        }
        if (gameObject.tag == "Cell")
        {
            int cycle = 0;
            float minerals = (boardSize.y - transform.position.y);
            if (minerals > 0) mineralBuffer += minerals / 48f * wi.mineralMultiplier;
            float sun = (transform.position.y);
            if (sun > 0) sun = sun / 32f * wi.sunMultiplier;
            if (mineralBuffer > maxMinerals) mineralBuffer = maxMinerals;
            bool cellInRange = false, corpseInRange = false;
            Collider2D C;
            while (cycle < maxCycle)
            {
                switch (behDNA[step].ID)
                {
                    case 0:
                        cycle += 5;
                        break;
                    case 1:
                        float sunPower = transform.position.y - Mathf.Floor(boardSize.y / 2f);
                        if (sunPower > 0) Health += sun * photoMultiplier;
                        cycle += 5;
                        break;
                    case 2:
                        Move((behDNA[step].info + rotation) % 4);
                        cycle += 5;
                        break;
                    case 3:
                        if (behDNA[step].info % 2 == 0) rotation++;
                        if (behDNA[step].info % 2 == 1) rotation--;
                        rotation %= 4;
                        cycle += 1;
                        break;
                    case 4:
                        Health += mineralBuffer * chemoMultiplier;
                        mineralBuffer = 0f;
                        cycle += 5;
                        break;
                    case 5:
                        Attack((behDNA[step].info + rotation) % 4);
                        cycle += 5;
                        break;
                    case 6:
                        step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 7:
                        for (int i = 0; i < 4; i++) if (cellInCoord(getCoordsInDirection(i)) != null && cellInCoord(getCoordsInDirection(i)).gameObject.tag == "Cell") cellInRange = true;
                        if (cellInRange) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 8:
                        for (int i = 0; i < 4; i++) if (cellInCoord(getCoordsInDirection(i)) != null && cellInCoord(getCoordsInDirection(i)).gameObject.tag == "Corpse") corpseInRange = true;
                        if (corpseInRange) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 9:
                        if (cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4)) != null && cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4)).gameObject.tag == "Cell") cellInRange = true;
                        if (cellInRange) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 10:
                        if (cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4)) != null && cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4)).gameObject.tag == "Corpse") cellInRange = true;
                        if (cellInRange) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 11:
                        if (transform.position.y < behDNA[step].info) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 12:
                        if (transform.position.y > behDNA[step].info) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 13:
                        C = cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4));
                        if (C != null && C.tag == "Cell")
                        {
                            if (testForParent(C.gameObject.GetComponent<CellBehavior>()) < 112) step = behDNA[step].adInfo - 1;
                        }
                        cycle += 1;
                        break;
                    case 14:
                        C = cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4));
                        if (C != null && C.tag == "Cell")
                        {
                            if (testForParent(C.gameObject.GetComponent<CellBehavior>()) >= 112) step = behDNA[step].adInfo - 1;
                        }
                        cycle += 1;
                        break;
                    case 15:
                        for (int i = 0; i < 4; i++)
                        {
                            C = cellInCoord(getCoordsInDirection(i));
                            if (C != null && C.tag == "Cell")
                            {
                                if (testForParent(C.gameObject.GetComponent<CellBehavior>()) < 112)
                                {
                                    step = behDNA[step].adInfo - 1;
                                    break;
                                }
                            }
                        }
                        cycle += 1;
                        break;
                    case 16:
                        for (int i = 0; i < 4; i++)
                        {
                            C = cellInCoord(getCoordsInDirection(i));
                            if (C != null && C.tag == "Cell")
                            {
                                if (testForParent(C.gameObject.GetComponent<CellBehavior>()) >= 112)
                                {
                                    step = behDNA[step].adInfo - 1;
                                    break;
                                }
                            }
                        }
                        cycle += 1;
                        break;
                    case 17:
                        if (register > behDNA[step].info % 8) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 18:
                        if (register < behDNA[step].info % 8) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 19:
                        if (register == behDNA[step].info % 8) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 20:
                        C = cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4));
                        if (C != null && C.tag == "Cell") register = C.gameObject.GetComponent<CellBehavior>().register;
                        cycle += 1;
                        break;
                    case 21:
                        C = cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4));
                        if (C != null && C.tag == "Cell") C.gameObject.GetComponent<CellBehavior>().register = register;
                        cycle += 1;
                        break;
                    case 22:
                        register = behDNA[step].info % 8;
                        cycle += 1;
                        break;
                    case 23:
                        if (Health >= MaxHealth / 2) Divide();
                        cycle += 5;
                        break;
                    case 24:
                        if (Health >= MaxHealth * behDNA[step].info / 64f) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 25:
                        if (Health <= MaxHealth * behDNA[step].info / 64f) step = behDNA[step].adInfo - 1;
                        cycle += 1;
                        break;
                    case 26:
                        Health -= Health * behDNA[step].info / 64f;
                        cycle += 5;
                        break;
                    case 27:
                        C = cellInCoord(getCoordsInDirection((behDNA[step].info + rotation) % 4));
                        if (C != null && C.tag == "Cell")
                        {
                            C.gameObject.GetComponent<CellBehavior>().Health += Health * behDNA[step].adInfo / 64f;
                            Health -= Health * behDNA[step].adInfo / 64f;
                        }
                        cycle += 5;
                        break;
                }
                step++;
                if (step >= behDNA.Length) step = 0;
            }

            if (Health >= MaxHealth) Divide();

            Health -= 1;
            if (Health <= 0) Die();
        }
        else if (gameObject.tag == "Corpse")
        {
            CorpseStep++;
            if (CorpseStep > stepsToCorpseDisapp) Destroy(gameObject);
            Move((int)directions.down);
        }
    }
    void Move(int direction)
    {
        Vector2 newCoords = getCoordsInDirection(direction);
        if (cellInCoord(newCoords) == null) transform.position = newCoords;
    }

    void Attack(int direction)
    {
        Vector2 targetCoords = getCoordsInDirection(direction);
        Collider2D target = cellInCoord(targetCoords);
        if (target != null && target.gameObject != gameObject && predatorPower >= target.GetComponent<CellBehavior>().predatorPower)
        {
            if (target.tag == "Cell")
            {
                Health += target.GetComponent<CellBehavior>().Health / 14.8f * predoMultiplier;
                target.GetComponent<CellBehavior>().isEated = true;
                step = behDNA[step].adInfo - 1;
            }
            else if (target.tag == "Corpse")
            {
                Health += 10f * trashMultiplier;
                target.GetComponent<CellBehavior>().isEated = true;
                step = behDNA[step].adInfo - 1;
            }
        }
    }

    Collider2D cellInCoord(Vector2 coords)
    {
        RaycastHit2D hit = Physics2D.Raycast(coords, Vector2.up, 0.1f);
        return hit.collider;
    }
    Vector2 getCoordsInDirection(int direction, int distance = 1)
    {
        Vector2 newCoords = new Vector2(transform.position.x, transform.position.y);
        switch (direction)
        {
            case 0:
                if (newCoords.y < boardSize.y)
                {
                    newCoords.y += distance;
                }
                break;
            case 1:
                newCoords.x = (int)transform.position.x + distance;
                if (newCoords.x > boardSize.x) newCoords.x -= boardSize.x + 1;
                break;
            case 2:
                if (newCoords.y > 0)
                {
                    newCoords.y -= distance;
                }
                break;
            case 3:
                newCoords.x = (int)transform.position.x - distance;
                if (newCoords.x < 0) newCoords.x += boardSize.x + 1;
                break;
        }
        return newCoords;
    }

    void Die()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        behColor = new Color(0, 0, 0);
        propColor = new Color(0, 0, 0);
        familyColor = new Color(0, 0, 0);
        gameObject.tag = "Corpse";
        CorpseStep = 0;
        Health = 0;
        rotation = 0;
        predatorPower = 0;
    }
    void Divide()
    {
        bool isDivide = false;
        int j = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            Vector2 testCoords = getCoordsInDirection((i + j) % 4);
            if (cellInCoord(testCoords) == null)
            {
                isDivide = true;
                GameObject newCell = Instantiate(obj, testCoords, transform.rotation);
                CellBehavior cb = newCell.GetComponent<CellBehavior>();
                cb.behDNA = new Gen[behDNAsize];
                cb.Health = Health / 2;
                cb.familyColor = familyColor;
                Health /= 2;
                for (int k = 0; k < cb.behDNA.Length; k++)
                {
                    Gen ng = new Gen();
                    ng.ID = behDNA[k].ID;
                    ng.info = behDNA[k].info;
                    ng.adInfo = behDNA[k].adInfo;
                    cb.behDNA[k] = ng;
                }
                for (int k = 0; k < cb.propDNA.Length; k++) cb.propDNA[k] = propDNA[k];
                cb.Mutate();
                break;
            }
        }
        if (!isDivide) Die();
    }
    public void Mutate()
    {
        for (int k = 0; k < behDNA.Length; k++)
        {
            if (Random.Range(0, 50) == 0)
            {
                Gen newGen = new Gen();
                newGen.ID = Random.Range(0, behCount);
                newGen.info = Random.Range(0, behDNAsize);
                newGen.adInfo = Random.Range(0, behDNAsize);
                behDNA[k] = newGen;
            }
        }

        for (int k = 0; k < propDNA.Length; k++)
        {
            if (Random.Range(0, 50) == 0) propDNA[k] = Random.Range(0, propCount);  
        }

    }

    int testForParent(CellBehavior g) // 0 - absolutely different; 128 - identical;
    {
        int ans = 0;
        for (int i = 0; i < behDNAsize; i++) if (behDNA[i].ID == g.behDNA[i].ID) ans++;
        for (int i = 0; i < propDNAsize; i++) if (propDNA[i] == g.propDNA[i]) ans++;
        return ans;
    }
}
