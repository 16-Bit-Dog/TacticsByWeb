using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityLookUpTable : MonoBehaviour

//localMatchIntermediateCS. - VAR's to deal with damage and such

{//AbilityLookUpTable.ALUT


    public static AbilityLookUpTable ALUT;

    public List<string> AbilityName;
    public List<string> AbilityBlurb;

    public List<int> RangeMin;
    public List<int> RangeMax;

    public List<int> RangeFuncType; //calc's range with linking to the RangeFuncCalc list of ranges calculators

    public List<Action<int, int, int, int, int, int, localMatchIntermediateCS.MapMakerVarsDat[][], Dictionary<Tuple<int, int>, GameObject>, localMatchIntermediateCS.CharDat>> RangeFuncCalc = new List<Action<int, int, int, int, int, int, localMatchIntermediateCS.MapMakerVarsDat[][], Dictionary<Tuple<int, int>, GameObject>, localMatchIntermediateCS.CharDat>>(); //

    public List<int> TurnCoolDown;

    public List<int> AffectFuncType; //fight type

    public List<Action<localMatchIntermediateCS.MapMakerVarsDat[][], localMatchIntermediateCS.CharDat, int, int, LocalMatchStart.MovePhaseC, bool, GameObject, GameObject, Dictionary<Tuple<int, int>, GameObject>>> AttackFunc = new List<Action<localMatchIntermediateCS.MapMakerVarsDat[][], localMatchIntermediateCS.CharDat, int, int, LocalMatchStart.MovePhaseC, bool, GameObject, GameObject, Dictionary<Tuple<int, int>, GameObject>>>(); //


    //utility funcs for universal use...:


    IEnumerator HurtAnimationBasic(localMatchIntermediateCS.CharDat CDat)
    {
        Animator animObj = CDat.CharObj.GetComponent<Animator>();
        if (animObj.GetBool("Selected") == false)
        {
            animObj.SetBool("Selected", true);
        }

        animObj.SetBool("Hurt", true);

        yield return null;

        while (animObj.GetBool("Hurt"))
        {
            yield return null;

        }

        if (animObj.GetBool("Selected") == true)
        {
            animObj.SetBool("Selected", false);
        }

        NewTurnDeathHandle(CDat);

        

        yield return null;



        //animation sets "Hurt" bool false when done


    }

    void UpdateHealthBar(localMatchIntermediateCS.CharDat HpUpdate)
    {
   //     LMS.UpdateCharStat(); --  not updating char stats...

        float tmp = (float)HpUpdate.Hp / (float)PriWLookup.PWLO.Hp[HpUpdate.PW];

        HpUpdate.HealthBar.transform.localScale = (new Vector3(tmp, 1.0f, 1.0f)); //

    }

    IEnumerator DiedAnimation(localMatchIntermediateCS.CharDat Cdat, Animator CdatAnim)
    {


        if (CdatAnim.GetBool("Selected") == false)
        {
            CdatAnim.SetBool("Selected", true);
        }

        CdatAnim.SetBool("Dead", true);

        Cdat.HealthBar.SetActive(false);
        Cdat.HealthBarBackLine.SetActive(false);

        yield return null;

        //  while (CdatAnim.GetBool("Dying") == false)
        //  {
        //      yield return null;

        //  }

    }
    void NewTurnDeathHandle(localMatchIntermediateCS.CharDat Cdat)
    {
        UpdateHealthBar(Cdat);

        if (Cdat.Hp <= 0)
        {
            if (Cdat.Dead == false)
            {

                Cdat.Hp = 0;

                Cdat.Dead = true;

                IEnumerator DA = DiedAnimation(Cdat, Cdat.CharObj.GetComponent<Animator>());

                StartCoroutine(DA);

                Cdat.HasTurn = false;

            }
            else
            {

                Cdat.Hp = 0;

            }
        }
        else
        {


        }
    }
    public void InitialTileAtkBuff(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        if (Cdat.AtkBuff > -6 || Cdat.AtkBuff < 6)
        { //max debuff of 10

            Cdat.AtkBuff += TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].AtkBuffInitial;
            if (Cdat.AtkBuff > 6)
            {
                Cdat.AtkBuff = 6;
            }
            else if (Cdat.AtkBuff < -6)
            {
                Cdat.AtkBuff = -6;
            }
        }

    }
    void InitialTileDefBuff(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        if (Cdat.DefBuff > -6 || Cdat.DefBuff < 6)
        { //max debuff of 10

            Cdat.DefBuff += TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].DefBuffInitial;
            if (Cdat.DefBuff > 6)
            {
                Cdat.DefBuff = 6;
            }
            else if (Cdat.DefBuff < -6)
            {
                Cdat.DefBuff = -6;
            }
        }

    }

    void InitialTileDmg(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        Cdat.Hp -= TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].DmgInitial;

    }

    void NewTileStatAdjustAll(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {
        InitialTileAtkBuff(Cdat, TilesArray);
        InitialTileDefBuff(Cdat, TilesArray);
        InitialTileDmg(Cdat, TilesArray);
        //heal, add heal buf, and then do dmg
    }
    //

    int RawDmgAtkDef(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat Attacker, localMatchIntermediateCS.CharDat Defender)
    {

        int AttackerEffectiveAtk = 0;
        int AttackerEffectiveDef = 0;

        int DefenderEffectiveAtk = 0;
        int DefenderEffectiveDef = 0;

        if (Attacker.CurW == 1)
        {
            AttackerEffectiveAtk = Attacker.Atk;
            AttackerEffectiveDef = Attacker.Def;
        }
        else if (Attacker.CurW == 2)
        {
            Attacker.ShowSecondary = true;
            AttackerEffectiveAtk = Attacker.Atk2;
            AttackerEffectiveDef = Attacker.Def2;
        }

        if (Defender.CurW == 1)
        {
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
        }
        else if (Defender.CurW == 2)
        {
            Defender.ShowSecondary = true;
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
        }

        int AttackerTileId = TilesArray[Attacker.PosY][Attacker.PosX].TileId;
        int DefenderTileId = TilesArray[Defender.PosY][Defender.PosX].TileId;

        int AttackerDamage = Math.Max(0, (AttackerEffectiveAtk + Attacker.AtkBuff + TileLookUp.TLU.Tiles[AttackerTileId].Atk) - (DefenderEffectiveDef + Defender.DefBuff + TileLookUp.TLU.Tiles[DefenderTileId].Def));

        return AttackerDamage;
    }

    //do damage based on inputted number and tile+around
    int RawDmgSpecialAtkDef(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat Attacker, localMatchIntermediateCS.CharDat Defender, int AtkDmg)
    {

        int AttackerEffectiveAtk = AtkDmg;
        int AttackerEffectiveDef = 0;

        int DefenderEffectiveAtk = 0;
        int DefenderEffectiveDef = 0;

        if (Attacker.CurW == 1)
        {
            //       AttackerEffectiveAtk = Attacker.Atk;
            AttackerEffectiveDef = Attacker.Def;
        }
        else if (Attacker.CurW == 2)
        {
            Attacker.ShowSecondary = true;

            //     AttackerEffectiveAtk = Attacker.Atk2;
            AttackerEffectiveDef = Attacker.Def2;
        }

        if (Defender.CurW == 1)
        {
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
        }
        else if (Defender.CurW == 2)
        {
            Defender.ShowSecondary = true;
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
        }

        int AttackerTileId = TilesArray[Attacker.PosY][Attacker.PosX].TileId;
        int DefenderTileId = TilesArray[Defender.PosY][Defender.PosX].TileId;

        int AttackerDamage = Math.Max(0, (AttackerEffectiveAtk + Attacker.AtkBuff + TileLookUp.TLU.Tiles[AttackerTileId].Atk) - (DefenderEffectiveDef + Defender.DefBuff + TileLookUp.TLU.Tiles[DefenderTileId].Def));

        return AttackerDamage;
    }

    void ClearTiles(Dictionary<Tuple<int, int>, GameObject> Tiles)
    {
        foreach (Tuple<int, int> i in Tiles.Keys)
        {

            Destroy(Tiles[i]);

        }
        Tiles.Clear();
    }



    //tile array, selected char, int for position y, int for position x, movePhase var, is move phase done, ActionAbilityInfoObj, CloseActionAbilityObj
    /*
     * Fight Code for Ability [range code is below fight code:
        if parameter is passed, the ability works - then continue to do the rest of code

        at the start I call MovePhase = 5 -->
      
        At middle I do logic - so a coroutine may do funny stuff and such to everything, that is when move phase 6 and such is called for the end:
     
        end I call an equivlent to: 
     
        LocalMatchStart.LMS.SelectedChar.HasTurn = false; - localMatchIntermediateCS.CharDat.HasTurn =false;
        LocalMatchStart.LMS.SelectedChar.MovedAlready = true; - localMatchIntermediateCS.CharDat.<ovedAlready = true;

        MovePhase = 6

        LocalMatchStart.LMS.CloseActionMenuAndDeselect(); <-- dunno how to do this, I suppose I call this when MovePhase is 6

    //may need to set char to null
     */


    //-2 Def Debuff Line through for 3:
    IEnumerator DefDebuffStraightLineAnimHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x)
    {
        tmpObjAnim.SetBool("Glare", true);

        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);
        //don't need to use a IEnum... but consitancy with code makes it good

        yield return null;
    }

    void DefDebuffStraightLineFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = StraightLineAtkRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    IEnumerator DefDebuffStraightLineRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles) //calc move done bool is also passed*
    {

        int MinRange = 0;
        int MaxRange = 0;

        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        //move phase 5 for now - but becomes 1 later
        LMS.MovePhase = 5;

        CalcMoveDone = true;

        SelectedChar.MovedAlready = true;


        //IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
        //StartCoroutine(routine);

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        IEnumerator tmpST = DefDebuffStraightLineAnimHandle(SelectedChar.CharObj, CharAnim, y, x);
        StartCoroutine(tmpST);

        if (MinRange == 1 && MaxRange == 1)
        {
            while (CharAnim.GetBool("Glare"))
            {
                yield return null;
            }
        }

        int EffAtk = 0;

        if (SelectedChar.CurW == 1)
        {
            EffAtk = SelectedChar.Atk;
        }
        else if (SelectedChar.CurW == 2)
        {
            EffAtk = SelectedChar.Atk2;
        }

        List<localMatchIntermediateCS.MapMakerVarsDat> EnemyTile = new List<localMatchIntermediateCS.MapMakerVarsDat>();

        if (x > SelectedChar.PosX)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX > SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (x < SelectedChar.PosX)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX < SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y > SelectedChar.PosY)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY > SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y < SelectedChar.PosY)
        { // I should make this a function... but copy pasta is faster   -  :'P
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY < SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }

        for (int i = 0; i < EnemyTile.Count; i++)
        {
            EnemyTile[i].CDat.DefBuff -= 3;
            EnemyTile[i].CDat.HpBuff -= 1;
        }



        SelectedChar.HasTurn = false;

        ClearTiles(TmpAbilityTiles);

        yield return null;

    }

    //-2 Atk Debuff Line through for 3:
    IEnumerator AtkDebuffStraightLineAnimHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x)
    {
        tmpObjAnim.SetBool("Glare", true);
        
        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);
        //don't need to use a IEnum... but consitancy with code makes it good

        yield return null;
    }

    void AtkDebuffStraightLineFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = StraightLineAtkRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    IEnumerator AtkDebuffStraightLineRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles) //calc move done bool is also passed*
    {

        int MinRange = 0;
        int MaxRange = 0;


        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        //move phase 5 for now - but becomes 1 later
        LMS.MovePhase = 5;

        CalcMoveDone = true;

        SelectedChar.MovedAlready = true;


        //IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
        //StartCoroutine(routine);

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        IEnumerator tmpST = AtkDebuffStraightLineAnimHandle(SelectedChar.CharObj, CharAnim, y, x);
        StartCoroutine(tmpST);

        if (MinRange == 1 && MaxRange == 1)
        {
            while (CharAnim.GetBool("Glare"))
            {
                yield return null;
            }
        }

        int EffAtk = 0;

        if (SelectedChar.CurW == 1)
        {
            EffAtk = SelectedChar.Atk;
        }
        else if (SelectedChar.CurW == 2)
        {
            EffAtk = SelectedChar.Atk2;
        }

        List<localMatchIntermediateCS.MapMakerVarsDat> EnemyTile = new List<localMatchIntermediateCS.MapMakerVarsDat>();

        if (x > SelectedChar.PosX)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX > SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (x < SelectedChar.PosX)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX < SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y > SelectedChar.PosY)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY > SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y < SelectedChar.PosY)
        { // I should make this a function... but copy pasta is faster   -  :'P
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY < SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }

        for (int i = 0; i < EnemyTile.Count; i++)
        {
            EnemyTile[i].CDat.AtkBuff -= 3;
            EnemyTile[i].CDat.HpBuff -= 1;
        }



        SelectedChar.HasTurn = false;

        ClearTiles(TmpAbilityTiles);

        yield return null;

    }


    //Straight line Dmg Func: - TODO: add perice animation thing
    IEnumerator StraightLineAtkAnimHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x, int MinRng, int MaxRng)
    {
        if (MinRng == 1 && MaxRng == 1)
        {
            tmpObjAnim.SetBool("ThinThrust", true);
        }
        else
        {
            tmpObjAnim.SetBool("BattleStart", true);
        }
        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);

        //don't need to use a IEnum... but consitancy with code makes it good

        yield return null;
    }

    void StraightLineAtkFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = StraightLineAtkRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    IEnumerator StraightLineAtkRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles) //calc move done bool is also passed*
    {

        int MinRange = 0;
        int MaxRange = 0;

        if (SelectedChar.CurW == 1)
        {
            SelectedChar.Ability1 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
            MinRange = SelectedChar.RngMinPri;
            MaxRange = SelectedChar.RngMaxPri;
        }
        else if (SelectedChar.CurW == 2)
        {
            SelectedChar.Ability2 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
            MaxRange = SelectedChar.RngMaxSec;
            MinRange = SelectedChar.RngMinSec;
        }


        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        //move phase 5 for now - but becomes 1 later
        LMS.MovePhase = 5;

        CalcMoveDone = true;

        SelectedChar.MovedAlready = true;


        //IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
        //StartCoroutine(routine);

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        IEnumerator tmpST = StraightLineAtkAnimHandle(SelectedChar.CharObj, CharAnim, y, x, MinRange, MaxRange);
        StartCoroutine(tmpST);

        if (MinRange == 1 && MaxRange == 1)
        {
            while (CharAnim.GetBool("ThinThrust"))
            {
                yield return null;
            }
        }
        else
        {
            while (CharAnim.GetBool("BattleStart"))
            {
                //animation sets BattleStart bool false when done

                if (CharAnim.GetBool("HitTheEnemy"))
                {

                    CharAnim.SetBool("HitTheEnemy", false);

                }
                yield return null;
            }
        }

        int EffAtk = 0;

        if (SelectedChar.CurW == 1)
        {
            EffAtk = SelectedChar.Atk;
        }
        else if (SelectedChar.CurW == 2)
        {
            EffAtk = SelectedChar.Atk2;
        }

        List<localMatchIntermediateCS.MapMakerVarsDat> EnemyTile = new List<localMatchIntermediateCS.MapMakerVarsDat>();

        if (x>SelectedChar.PosX) 
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX>SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (x < SelectedChar.PosX)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosX < SelectedChar.PosX)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y > SelectedChar.PosY)
        {
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY > SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }
        if (y < SelectedChar.PosY)
        { // I should make this a function... but copy pasta is faster   -  :'P
            foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
            {
                if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team != SelectedChar.team && TilesArray[i.Item1][i.Item2].CDat.PosY < SelectedChar.PosY)
                {
                    EnemyTile.Add(TilesArray[i.Item1][i.Item2]);
                }
            }
        }

        for(int i = 0; i < EnemyTile.Count; i++)
        {
            EnemyTile[i].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, EnemyTile[i].CDat, EffAtk);
            IEnumerator TMP = HurtAnimationBasic(EnemyTile[i].CDat);
            StartCoroutine(TMP);
        }

        for (int i = 0; i < EnemyTile.Count; i++)
        {
            Animator tmpAnimEn = EnemyTile[i].CDat.CharObj.GetComponent<Animator>();
            while (tmpAnimEn.GetBool("Hurt"))
            {
                yield return null;
            }
            while (EnemyTile[i].CDat.Dead == true && tmpAnimEn.GetBool("Dying") == false)
            {
                yield return null;
            }
            if (tmpAnimEn.GetBool("Selected") == true)
            {
                tmpAnimEn.SetBool("Selected", false);
            }
        }


        SelectedChar.HasTurn = false;

        ClearTiles(TmpAbilityTiles);

        yield return null;

        }


  
    



    //Sparrowing speech - buff mov of near
    IEnumerator TwoMovBuffAndSelfHandle(GameObject tmpObj, Animator tmpObjAnim/*, int y, int x, localMatchIntermediateCS.CharDat SelectedChar*/)
    {

        tmpObjAnim.SetBool("SurveyLookS", true);

        yield return null;

    }


    IEnumerator TwoMovBuffAndSelfRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        //buff in range if tmp tiles array
        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        int EffAtk = 0;

        if (SelectedChar.CurW == 1)
        {
            EffAtk = SelectedChar.Atk;
        }
        else if (SelectedChar.CurW == 2)
        {
            EffAtk = SelectedChar.Atk2;
        }

        LMS.MovePhase = 5;

        CalcMoveDone = true;



        SelectedChar.MovedAlready = true;

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        //war cry anim
        IEnumerator tmpST = TwoDefBuffAndSelfHandle(SelectedChar.CharObj, CharAnim/*, y, x, SelectedChar*/);
        StartCoroutine(tmpST);

        TilesArray[y][x].CDat.MovBuff += 2;

        foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
        {
            if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team == SelectedChar.team)
            {
                TilesArray[i.Item1][i.Item2].CDat.MovBuff += 2;
            }


        }

        ClearTiles(TmpAbilityTiles);

        while (CharAnim.GetBool("SurveyLookS"))
        {
            yield return null;
        }

        SelectedChar.HasTurn = false;


        yield return null;
    }

    void TwoMovBuffAndSelfFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = TwoAtkBuffAndSelfRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }


    //Pious Stance - buff def of near 
    IEnumerator TwoDefBuffAndSelfHandle(GameObject tmpObj, Animator tmpObjAnim/*, int y, int x, localMatchIntermediateCS.CharDat SelectedChar*/)
    {

        tmpObjAnim.SetBool("PDefSpeech", true);

        yield return null;

    }


    IEnumerator TwoDefBuffAndSelfRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        //buff in range if tmp tiles array
        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        int EffAtk = 0;

        if (SelectedChar.CurW == 1)
        {
            EffAtk = SelectedChar.Atk;
        }
        else if (SelectedChar.CurW == 2)
        {
            EffAtk = SelectedChar.Atk2;
        }

        LMS.MovePhase = 5;

        CalcMoveDone = true;



        SelectedChar.MovedAlready = true;

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        //war cry anim
        IEnumerator tmpST = TwoDefBuffAndSelfHandle(SelectedChar.CharObj, CharAnim/*, y, x, SelectedChar*/);
        StartCoroutine(tmpST);

        TilesArray[y][x].CDat.DefBuff += 3;

        foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
        {
            if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team == SelectedChar.team)
            {
                TilesArray[i.Item1][i.Item2].CDat.DefBuff += 2;
            }


        }

        ClearTiles(TmpAbilityTiles);

        while (CharAnim.GetBool("PDefSpeech"))
        {
            yield return null;
        }

        SelectedChar.HasTurn = false;


        yield return null;
    }

    void TwoDefBuffAndSelfFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = TwoAtkBuffAndSelfRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }


    //Invigorating War-Cry - and others use this

    IEnumerator TwoAtkBuffAndSelfHandle(GameObject tmpObj, Animator tmpObjAnim/*, int y, int x, localMatchIntermediateCS.CharDat SelectedChar*/)
    {

        tmpObjAnim.SetBool("WarCry", true);

        yield return null;

    }


    IEnumerator TwoAtkBuffAndSelfRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        //buff in range if tmp tiles array
            CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
            ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

            int EffAtk = 0;

            if (SelectedChar.CurW == 1)
            {
                EffAtk = SelectedChar.Atk;
            }
            else if (SelectedChar.CurW == 2)
            {
                EffAtk = SelectedChar.Atk2;
            }

            LMS.MovePhase = 5;

            CalcMoveDone = true;

        

            SelectedChar.MovedAlready = true;
        
            Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

            //war cry anim
            IEnumerator tmpST = TwoAtkBuffAndSelfHandle(SelectedChar.CharObj, CharAnim/*, y, x, SelectedChar*/);
            StartCoroutine(tmpST);

            TilesArray[y][x].CDat.AtkBuff += 3;

        foreach (Tuple<int, int> i in TmpAbilityTiles.Keys)
        {
            if (TilesArray[i.Item1][i.Item2].CDat != null && TilesArray[i.Item1][i.Item2].CDat.team == SelectedChar.team)
            {
                TilesArray[i.Item1][i.Item2].CDat.AtkBuff += 2;
            }
            

        }
        
        ClearTiles(TmpAbilityTiles);

        while (CharAnim.GetBool("WarCry"))
        {
            yield return null;
        }

        SelectedChar.HasTurn = false;


        yield return null;
    }

    void TwoAtkBuffAndSelfFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = TwoAtkBuffAndSelfRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    //DeepWound - inflict HP debuff as if posion of 5 and do reg atk: - remember these can be mixed and matched with other stats like a bow
    
    IEnumerator DeepWoundHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x, GameObject tmpObj2, Animator tmpObjAnim2, localMatchIntermediateCS.CharDat SelectedChar, localMatchIntermediateCS.CharDat Defender)
    {

        tmpObjAnim.SetBool("BattleStart", true);

        if (tmpObjAnim2.GetBool("Selected") == false)
        {
            tmpObjAnim2.SetBool("Selected", true);
        }

        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        Vector3 PosC = tmpObj2.transform.position;
        Vector3 PosD = new Vector3(4 * SelectedChar.PosX, tmpObj2.transform.position.y, 4 * SelectedChar.PosY);


        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);

        tmpObj2.transform.rotation = Quaternion.LookRotation((PosC - PosD));
        tmpObj2.transform.RotateAround(tmpObj2.transform.position, Vector3.up, -90);

        while (tmpObjAnim.GetBool("BattleStart"))
        {
            //animation sets BattleStart bool false when done

            if (tmpObjAnim.GetBool("HitTheEnemy"))
            {

                UpdateHealthBar(Defender);

                tmpObjAnim2.SetBool("Hurt", true);

                tmpObjAnim.SetBool("HitTheEnemy", false);

                yield return null;

            }
            yield return null;
        }

        while (tmpObjAnim2.GetBool("Hurt"))
        {
            yield return null;
        }

        if (tmpObjAnim2.GetBool("Selected") == true)
        {
            tmpObjAnim2.SetBool("Selected", false);
        }

        NewTurnDeathHandle(Defender);

        yield return null;

    }


    IEnumerator DeepWoundRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {

        if (TilesArray[y][x].CDat != null) //only continue if enemy is clicked
        {
            ClearTiles(TmpAbilityTiles);

            CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
            ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

            int EffAtk = 0;

            if (SelectedChar.CurW == 1)
            {
                EffAtk = SelectedChar.Atk;
            }
            else if (SelectedChar.CurW == 2)
            {
                EffAtk = SelectedChar.Atk2;
            }

            LMS.MovePhase = 5;

            CalcMoveDone = true;

            SelectedChar.MovedAlready = true;


            Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

            Animator CharTargetAnim = TilesArray[y][x].CDat.CharObj.GetComponent<Animator>();

            IEnumerator tmpST = DeepWoundHandle(SelectedChar.CharObj, CharAnim, y, x, TilesArray[y][x].CDat.CharObj, CharTargetAnim, SelectedChar, TilesArray[y][x].CDat);
            StartCoroutine(tmpST);

            TilesArray[y][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x].CDat, EffAtk);
            TilesArray[y][x].CDat.HpBuff -= 5;


            yield return null;

            while (SelectedChar.Dead == true && CharAnim.GetBool("Dying") == false)
            {
                yield return null;
            }

            while (CharTargetAnim.GetBool("Hurt"))
            {
                yield return null;
            }
            while (TilesArray[y][x].CDat.Dead == true && CharTargetAnim.GetBool("Dying") == false)
            {
                yield return null;
            }

            SelectedChar.HasTurn = false;

        }

        yield return null;
    }

    void DeepWoundFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = DeepWoundRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }




    //Curved Slash - switch places and end turn - after you attack you switch with who you attacked with (no back stab damage)



    IEnumerator CurvedSlashHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x, GameObject tmpObj2, Animator tmpObjAnim2, localMatchIntermediateCS.CharDat SelectedChar)
    {

        tmpObjAnim.SetBool("BattleStart", true);

        
        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        Vector3 PosC = tmpObj2.transform.position;
        Vector3 PosD = new Vector3(4 * SelectedChar.PosX, tmpObj2.transform.position.y, 4 * SelectedChar.PosY);


        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);

        float testc = 0.0f;

        while (testc < 1)
        {
            testc += Time.deltaTime / 0.5f;

            tmpObj.transform.position = Vector3.Lerp(PosA, PosB, testc); //1 second

            yield return null;
        }

        
        tmpObj2.transform.rotation = Quaternion.LookRotation((PosC - PosD));
        tmpObj2.transform.RotateAround(tmpObj2.transform.position, Vector3.up, -90);

        testc = 0.0f;

        while (testc < 1)
        {
            testc += Time.deltaTime / 0.5f;

            tmpObj2.transform.position = Vector3.Lerp(PosC, PosD, testc); //1 second

            yield return null;
        }

        tmpObj2.transform.position = PosD;

        while (tmpObjAnim.GetBool("BattleStart"))
        {
            //animation sets BattleStart bool false when done

            if (tmpObjAnim.GetBool("HitTheEnemy"))
            {

                tmpObjAnim.SetBool("HitTheEnemy", false);

                yield return null;

            }
            yield return null;
        }

        yield return null;

    }


    IEnumerator CurvedSlashRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {

        if(TilesArray[y][x].CDat != null) //only continue if enemy is clicked
        {
            ClearTiles(TmpAbilityTiles);

            CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
            ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

            int EffAtk = 0;

            if (SelectedChar.CurW == 1)
            {
                EffAtk = SelectedChar.Atk / 2 * 3;
            }
            else if (SelectedChar.CurW == 2)
            {
                EffAtk = SelectedChar.Atk2 / 2 * 3;
            }

            LMS.MovePhase = 5;

            CalcMoveDone = true;

            SelectedChar.MovedAlready = true;

            
            Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

            Animator CharTargetAnim = TilesArray[y][x].CDat.CharObj.GetComponent<Animator>();

            IEnumerator tmpST = CurvedSlashHandle(SelectedChar.CharObj, CharAnim, y, x, TilesArray[y][x].CDat.CharObj, CharTargetAnim, SelectedChar);
            StartCoroutine(tmpST);

            TilesArray[y][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x].CDat, EffAtk);
            //after dmg is tile move affects

            localMatchIntermediateCS.CharDat Tmp = TilesArray[y][x].CDat;
            int OGx = SelectedChar.PosX;
            int OGy = SelectedChar.PosY;

            TilesArray[y][x].CDat = SelectedChar;

            TilesArray[SelectedChar.PosY][SelectedChar.PosX].CDat = null;

            SelectedChar.PosY = y;
            SelectedChar.PosX = x;

            NewTileStatAdjustAll(SelectedChar, TilesArray);

            NewTurnDeathHandle(SelectedChar);

            //other guy being attacked gets handled*
            TilesArray[OGy][OGx].CDat = Tmp;

            TilesArray[OGy][OGx].CDat.PosY = OGy;
            TilesArray[OGy][OGx].CDat.PosX = OGx;

            NewTileStatAdjustAll(TilesArray[OGy][OGx].CDat, TilesArray);

            NewTurnDeathHandle(TilesArray[OGy][OGx].CDat);


            yield return null;

            while (SelectedChar.Dead == true && CharAnim.GetBool("Dying") == false)
            {
                yield return null;
            }

            while (CharTargetAnim.GetBool("Hurt"))
            {
                yield return null;
            }
            while (TilesArray[OGy][OGx].CDat.Dead == true && CharTargetAnim.GetBool("Dying") == false)
            {
                yield return null;
            }
            SelectedChar.HasTurn = false;

        }

        yield return null;
    }

    void CurvedSlashFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = CurvedSlashRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }


    //meteor slam - jump and slam down on your current space to hit 4 around [maybe make all plains into desert 3]
    IEnumerator MeteorSlamAnimHandle(GameObject tmpObj, Animator tmpObjAnim, int y, int x)
    {
        tmpObjAnim.SetBool("MeteorSlam", true);

        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB));
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90);

        //don't need to use a IEnum... but consitancy with code makes it good

        yield return null;
    }

    void MeteorSlamFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = MeteorSlamRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    IEnumerator MeteorSlamRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles) //calc move done bool is also passed*
    {

       

        if (SelectedChar.CurW == 1)
        {
            SelectedChar.Ability1 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
        }
        else if (SelectedChar.CurW == 2)
        {

            SelectedChar.Ability2 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
        }

        ClearTiles(TmpAbilityTiles);

        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        //move phase 5 for now - but becomes 1 later
        LMS.MovePhase = 5;

        CalcMoveDone = true;

        SelectedChar.MovedAlready = true;

        
        //IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
        //StartCoroutine(routine);

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        IEnumerator tmpST = MeteorSlamAnimHandle(SelectedChar.CharObj, CharAnim, y, x);
        StartCoroutine(tmpST);

        while (CharAnim.GetBool("MeteorSlam"))
        {
            yield return null;
        }

        y = SelectedChar.PosY;
        x = SelectedChar.PosX;

        if (SelectedChar.Hp > 0)
        {

            int EffAtk = 0;

            if (SelectedChar.CurW == 1)
            {
                EffAtk = SelectedChar.Atk;
            }
            else if (SelectedChar.CurW == 2)
            {
                EffAtk = SelectedChar.Atk2;
            }

            //TODO: play coroutine spin animation and lerp to position, whilst playing animation for spin attack animation

            //damage to the 4 spaces around if it has people regadless of team - do damage part is 4 ifs and not null to do rawdmg

            if (TilesArray[y + 1][x].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y + 1][x].CDat.Dead == false)
            {
                TilesArray[y + 1][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y + 1][x].CDat, EffAtk);

                IEnumerator TMP = HurtAnimationBasic(TilesArray[y + 1][x].CDat);
                StartCoroutine(TMP);
                //TODO: play couroutine hurt animation
                //  NewTurnDeathHandle(TilesArray[y][x - 1].CDat);

            }
            if (TilesArray[y - 1][x].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y - 1][x].CDat.Dead == false)
            {

                TilesArray[y - 1][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y - 1][x].CDat, EffAtk);

                IEnumerator TMP = HurtAnimationBasic(TilesArray[y - 1][x].CDat);
                StartCoroutine(TMP);
            }
            if (TilesArray[y][x + 1].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y][x + 1].CDat.Dead == false)
            {
                TilesArray[y][x + 1].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x + 1].CDat, EffAtk);

                IEnumerator TMP = HurtAnimationBasic(TilesArray[y][x + 1].CDat);
                StartCoroutine(TMP);
            }
            if (TilesArray[y][x - 1].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y][x - 1].CDat.Dead == false)
            {
                TilesArray[y][x - 1].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x - 1].CDat, EffAtk);

                IEnumerator TMP = HurtAnimationBasic(TilesArray[y][x - 1].CDat);
                StartCoroutine(TMP);

            }

            yield return null;

            //stall until all done dying, hurting, and spin animation, then set move to reg move phase
            while (SelectedChar.Dead == true && CharAnim.GetBool("Dying") == false)
            {

                yield return null;
            }
            

            if (TilesArray[y + 1][x].CDat != null)
            {

                Animator AroundCharAnim1 = TilesArray[y + 1][x].CDat.CharObj.GetComponent<Animator>();

                while (AroundCharAnim1.GetBool("Hurt"))
                {
                    yield return null;
                }

                while (TilesArray[y + 1][x].CDat.Dead == true && AroundCharAnim1.GetBool("Dying") == false)
                {
                    yield return null;
                }

            }
            if (TilesArray[y - 1][x].CDat != null)
            {
                Animator AroundCharAnim2 = TilesArray[y - 1][x].CDat.CharObj.GetComponent<Animator>();

                while (AroundCharAnim2.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y - 1][x].CDat.Dead == true && AroundCharAnim2.GetBool("Dying") == false)
                {
                    yield return null;
                }
            }

            if (TilesArray[y][x + 1].CDat != null)
            {

                Animator AroundCharAnim3 = TilesArray[y][x + 1].CDat.CharObj.GetComponent<Animator>();
                while (AroundCharAnim3.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y][x + 1].CDat.Dead == true && AroundCharAnim3.GetBool("Dying") == false)
                {
                    
                    yield return null;
                }
            }
            if (TilesArray[y][x - 1].CDat != null)
            {
                Animator AroundCharAnim4 = TilesArray[y][x - 1].CDat.CharObj.GetComponent<Animator>();
                while (AroundCharAnim4.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y][x - 1].CDat.Dead == true && AroundCharAnim4.GetBool("Dying") == false)
                {
                    yield return null;
                }
            }

            SelectedChar.HasTurn = false;

        }


        yield return null;

    }

    //SwiftStrike - attack with a circle-ular strike based on final pos - then act as if you moved (attack is allowed)

    IEnumerator SwiftStrikeAnimationAndMove(GameObject tmpObj, Animator tmpObjAnim, int y, int x) 
    {
        tmpObjAnim.SetBool("SpinSlash", true);

        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB)); 
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90); 

        float testc = 0.0f;

        while (testc < 1)
        {
            testc += Time.deltaTime / 0.5f;

            tmpObj.transform.position = Vector3.Lerp(PosA, PosB, testc); //1 second

            yield return null;
        }

        tmpObj.transform.position = PosB;
        tmpObjAnim.SetBool("SpinSlash", false);
        tmpObjAnim.SetBool("SpinSlashSlash", true);

    }

    void SwiftStrikeFunc(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles)
    {
        IEnumerator CR = SwiftStrikeRoutine(TilesArray, SelectedChar, y, x, LMS, CalcMoveDone, ActionAbilityInfoObj, CloseActionAbilityObj, TmpAbilityTiles);

        StartCoroutine(CR);
    }

    IEnumerator SwiftStrikeRoutine(localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray, localMatchIntermediateCS.CharDat SelectedChar, int y, int x, LocalMatchStart.MovePhaseC LMS, bool CalcMoveDone, GameObject ActionAbilityInfoObj, GameObject CloseActionAbilityObj, Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles) //calc move done bool is also passed*
    {
        if (SelectedChar.CurW == 1) {
            SelectedChar.Ability1 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
        }
        else if (SelectedChar.CurW == 2)
        {
            SelectedChar.Ability2 = AbilityLookUpTable.ALUT.TurnCoolDown[PriWLookup.PWLO.AbilityNumber[SelectedChar.PW]];
        }

        ClearTiles(TmpAbilityTiles);

        CloseActionAbilityObj.SetActive(false); //now you are locked inside the move phase 5
        ActionAbilityInfoObj.SetActive(false); //now you are locked inside the move phase 5

        //move phase 5 for now - but becomes 1 later
        LMS.MovePhase = 5;

        CalcMoveDone = true;

        SelectedChar.MovedAlready = true;

        //IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
        //StartCoroutine(routine);

        TilesArray[y][x].CDat = SelectedChar;

        TilesArray[SelectedChar.PosY][SelectedChar.PosX].CDat = null;

        SelectedChar.PosY = y;
        SelectedChar.PosX = x;

        Animator CharAnim = SelectedChar.CharObj.GetComponent<Animator>();

        NewTileStatAdjustAll(SelectedChar, TilesArray);

        NewTurnDeathHandle(SelectedChar);

        
        if (SelectedChar.Hp > 0)
        {

            IEnumerator tmpST = SwiftStrikeAnimationAndMove(SelectedChar.CharObj, CharAnim, y, x);
            StartCoroutine(tmpST);

            while (CharAnim.GetBool("SpinSlash"))
            {
                yield return null;
            }

            while (CharAnim.GetBool("SpinSlashSlash"))
            {
                yield return null;
            }


            int EffAtk = 0;

            if(SelectedChar.CurW == 1)
            {
                EffAtk = SelectedChar.Atk/2;
            }
            else if(SelectedChar.CurW == 2)
            {
                EffAtk = SelectedChar.Atk2/2;
            }

            //TODO: play coroutine spin animation and lerp to position, whilst playing animation for spin attack animation

            //damage to the 4 spaces around if it has people regadless of team - do damage part is 4 ifs and not null to do rawdmg

            if (TilesArray[y + 1][x].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y + 1][x].CDat.Dead == false)
            {
                TilesArray[y + 1][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y + 1][x].CDat, EffAtk);

                IEnumerator TMP = HurtAnimationBasic(TilesArray[y + 1][x].CDat);
                StartCoroutine(TMP);
                //TODO: play couroutine hurt animation
                //  NewTurnDeathHandle(TilesArray[y][x - 1].CDat);

            }
            if (TilesArray[y - 1][x].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y - 1][x].CDat.Dead == false)
            {

                TilesArray[y - 1][x].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y - 1][x].CDat, EffAtk);
                
                IEnumerator TMP = HurtAnimationBasic(TilesArray[y - 1][x].CDat);
                StartCoroutine(TMP);
            }
            if (TilesArray[y][x + 1].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y][x + 1].CDat.Dead == false)
            {
                TilesArray[y][x + 1].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x + 1].CDat, EffAtk);
                
                IEnumerator TMP = HurtAnimationBasic(TilesArray[y][x + 1].CDat);
                StartCoroutine(TMP);
            }
            if (TilesArray[y][x - 1].CDat != null && /*TilesArray[y][x].CDat.team != LMS.SelectedChar.team &&*/ TilesArray[y][x - 1].CDat.Dead == false)
            {
                TilesArray[y][x - 1].CDat.Hp -= RawDmgSpecialAtkDef(TilesArray, SelectedChar, TilesArray[y][x - 1].CDat, EffAtk);
                
                IEnumerator TMP = HurtAnimationBasic(TilesArray[y][x - 1].CDat);
                StartCoroutine(TMP);

            }

            yield return null;

            //stall until all done dying, hurting, and spin animation, then set move to reg move phase
            while (SelectedChar.Dead == true && CharAnim.GetBool("Dying") == false)
            {
                yield return null;
            }


            if (TilesArray[y + 1][x].CDat != null)
            {

                Animator AroundCharAnim1 = TilesArray[y + 1][x].CDat.CharObj.GetComponent<Animator>();

                while (AroundCharAnim1.GetBool("Hurt"))
                {
                    yield return null;
                }

                while (TilesArray[y + 1][x].CDat.Dead == true && AroundCharAnim1.GetBool("Dying") == false)
                {
                    yield return null;
                }
            
            }

            if (TilesArray[y - 1][x].CDat != null)
            {
                Animator AroundCharAnim2 = TilesArray[y - 1][x].CDat.CharObj.GetComponent<Animator>();

                while (AroundCharAnim2.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y - 1][x].CDat.Dead == true && AroundCharAnim2.GetBool("Dying") == false)
                {
                    yield return null;
                }
            }

            if (TilesArray[y][x + 1].CDat != null)
            {
                Animator AroundCharAnim3 = TilesArray[y][x + 1].CDat.CharObj.GetComponent<Animator>();
                while (AroundCharAnim3.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y][x + 1].CDat.Dead == true && AroundCharAnim3.GetBool("Dying") == false)
                {
                    yield return null;
                }
            }

            if (TilesArray[y][x - 1].CDat != null)
            {
                Animator AroundCharAnim4 = TilesArray[y][x - 1].CDat.CharObj.GetComponent<Animator>();
                while (AroundCharAnim4.GetBool("Hurt"))
                {
                    yield return null;
                }
                while (TilesArray[y][x - 1].CDat.Dead == true && AroundCharAnim4.GetBool("Dying") == false)
                {
                    yield return null;
                }
            }

        }

        LMS.MovePhase = 1;
        yield return null;

    }
    //


    /// 
    /// Range Code is below:
    /// 


    //I have a seperate calc in case I need to use weird params
    void AbilityDFSBlockNone(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityDFSBlockNoneNextTile(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityDFSBlockNoneNextTile(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityDFSBlockNoneNextTile(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityDFSBlockNoneNextTile(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }
    void AbilityDFSBlockNoneNextTile(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {

            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityDFSBlockNone(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityDFSBlockNone(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        
    }


    void AbilityDFSBlockWall(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityDFSBlockWallNextTile(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityDFSBlockWallNextTile(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityDFSBlockWallNextTile(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityDFSBlockWallNextTile(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }

    void AbilityDFSBlockWallNextTile(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (TileLookUp.TLU.Tiles[TileArray[y][x].TileId].MoveCost == 100) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityDFSBlockWall(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityDFSBlockWall(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }

    void AbilityDFSBlockEnemy(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityDFSBlockEnemyNextTile(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityDFSBlockEnemyNextTile(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityDFSBlockEnemyNextTile(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityDFSBlockEnemyNextTile(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }
    void AbilityDFSBlockEnemyNextTile(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (TileArray[y][x].CDat != null && TileArray[y][x].CDat.team != SelectedCharDat.team && TileArray[y][x].CDat.Dead == false) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityDFSBlockEnemy(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityDFSBlockEnemy(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }

    void AbilityDFSBlockEnemyWall(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityDFSBlockEnemyWallNextTile(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityDFSBlockEnemyWallNextTile(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityDFSBlockEnemyWallNextTile(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityDFSBlockEnemyWallNextTile(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }
    void AbilityDFSBlockEnemyWallNextTile(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (TileArray[y][x].CDat != null && TileArray[y][x].CDat.team != SelectedCharDat.team && TileArray[y][x].CDat.Dead == false || TileLookUp.TLU.Tiles[TileArray[y][x].TileId].MoveCost == 100) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityDFSBlockEnemyWall(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityDFSBlockEnemyWall(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }

    void AbilityDFSBlockMove(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {

        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityDFSBlockMoveNextTile(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityDFSBlockMoveNextTile(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityDFSBlockMoveNextTile(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityDFSBlockMoveNextTile(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }

    void AbilityDFSBlockMoveNextTile(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (TileArray[y][x].CDat != null && TileArray[y][x].CDat.team != SelectedCharDat.team && TileArray[y][x].CDat.Dead == false || TileLookUp.TLU.Tiles[TileArray[y][x].TileId].MoveCost == 100 || TileLookUp.TLU.Tiles[TileArray[y][x].TileId].MoveCost == 99) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityDFSBlockMove(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityDFSBlockEnemyWall(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }

    void AbilityStraightLineWallBlock(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {

        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityStraightLineNextTileWallBlock(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityStraightLineNextTileWallBlock(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityStraightLineNextTileWallBlock(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityStraightLineNextTileWallBlock(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }

    void AbilityStraightLineNextTileWallBlock(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (TileLookUp.TLU.Tiles[TileArray[y][x].TileId].MoveCost == 100 || x != startX && y != startY) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityStraightLineWallBlock(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityStraightLineWallBlock(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }

    void AbilityStraightLineWallZERO(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {

        if (MaxRange > 0)
        {
            if (x > 0)
            {
                AbilityStraightLineNextTileZEROBlock(x - 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (x < TileArray[0].GetLength(0) - 1)
            {
                AbilityStraightLineNextTileZEROBlock(x + 1, y, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y > 0)
            {
                AbilityStraightLineNextTileZEROBlock(x, y - 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
            if (y < TileArray.GetLength(0) - 1)
            {
                AbilityStraightLineNextTileZEROBlock(x, y + 1, MaxRange, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);
            }
        }
    }

    void AbilityStraightLineNextTileZEROBlock(int x, int y, int MaxRange, int MinRange, int startX, int startY, localMatchIntermediateCS.MapMakerVarsDat[][] TileArray, Dictionary<Tuple<int, int>, GameObject> RangeTile, localMatchIntermediateCS.CharDat SelectedCharDat)
    {
        if (x != startX && y != startY) //muh boi, da line is not straight!
        {

            //stops

        }
        else
        {
            if (MaxRange >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!RangeTile.ContainsKey(Tuple.Create(y, x)))
                {
                    RangeTile[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AbilityStraightLineWallZERO(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost


            }
            else if (MaxRange - 1 >= 0)
            {

                AbilityStraightLineWallZERO(x, y, MaxRange - 1, MinRange, startX, startY, TileArray, RangeTile, SelectedCharDat);  //minus tile ID mov cost

            }
        }
    }


    // Start is called before the first frame update
    void Awake()
    {

        RangeFuncCalc.Add(AbilityDFSBlockNone);
        RangeFuncCalc.Add(AbilityDFSBlockWall);
        RangeFuncCalc.Add(AbilityDFSBlockEnemy);
        RangeFuncCalc.Add(AbilityDFSBlockEnemyWall);
        RangeFuncCalc.Add(AbilityDFSBlockMove);
        RangeFuncCalc.Add(AbilityStraightLineWallBlock);
        RangeFuncCalc.Add(AbilityStraightLineWallZERO);

        AttackFunc.Add(SwiftStrikeFunc);
        AttackFunc.Add(MeteorSlamFunc);
        AttackFunc.Add(CurvedSlashFunc);
        AttackFunc.Add(DeepWoundFunc);
        AttackFunc.Add(TwoAtkBuffAndSelfFunc);
        AttackFunc.Add(TwoDefBuffAndSelfFunc);
        AttackFunc.Add(TwoMovBuffAndSelfFunc);
        AttackFunc.Add(StraightLineAtkFunc);
        AttackFunc.Add(AtkDebuffStraightLineFunc);
        AttackFunc.Add(DefDebuffStraightLineFunc);


        if (ALUT == null)
        {
            ALUT = this;
        }
        else if (ALUT != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(ALUT);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
