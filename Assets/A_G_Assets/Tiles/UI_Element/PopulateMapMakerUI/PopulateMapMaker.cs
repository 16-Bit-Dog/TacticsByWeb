using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateMapMaker : MonoBehaviour
{//remember to instantiate new objects such that it follows the row logic
	
	public GameObject ClearChar;
	public GameObject BlueChar;
	public GameObject RedChar;
	public GameObject YellowChar;
	public GameObject GreenChar;
	public GameObject PurpleChar;

	public GameObject Monument1;
	public GameObject Monument2;
	public GameObject Monument3;

	public GameObject Forest1; // This is our prefab object that will be exposed in the inspector
	public GameObject Forest2;
	public GameObject Forest3;
	
	public GameObject Plains1;
	public GameObject Plains2;
	public GameObject Plains3;
	
	public GameObject Rock1;
	public GameObject Rock2;
	public GameObject Rock3;

	public GameObject Grass1;
	public GameObject Grass2;
	public GameObject Grass3;

	public GameObject StoneWall1;
	public GameObject StoneWall2;
	public GameObject StoneWall3;

	public GameObject Bush1;
	public GameObject Bush2;
	public GameObject Bush3;

	public GameObject HealthBush1;
	public GameObject HealthBush2;
	public GameObject HealthBush3;

	public GameObject InvigoratingBush1;
	public GameObject InvigoratingBush2;
	public GameObject InvigoratingBush3;

	public GameObject FortifyingBush1;
	public GameObject FortifyingBush2;
	public GameObject FortifyingBush3;

	public GameObject ReflexBush1;
	public GameObject ReflexBush2;
	public GameObject ReflexBush3;
	
	public GameObject QuickSand1;
	public GameObject QuickSand2;
	public GameObject QuickSand3;

	public GameObject Mountain1;
	public GameObject Mountain2;
	public GameObject Mountain3;

	public GameObject Spike1;
	public GameObject Spike2;
	public GameObject Spike3;

	public GameObject PureWater1;
	public GameObject PureWater2;
	public GameObject PureWater3;

	public GameObject Shoal1;
	public GameObject Shoal2;
	public GameObject Shoal3;

	

	public int numberToCreate; // number of objects to create. Exposed in inspector

	void Start()
	{
		Populate();
	}

	void Update()
	{

	}

	void Populate()
	{
		// Create new instances of our prefab until we've created as many as we specified
		ClearChar = Instantiate(ClearChar, transform);
		BlueChar = Instantiate(BlueChar, transform);
		RedChar = Instantiate(RedChar, transform);
		YellowChar = Instantiate(YellowChar, transform);
		GreenChar = Instantiate(GreenChar, transform);
		PurpleChar = Instantiate(PurpleChar, transform);

		Monument1 = Instantiate(Monument1, transform);
		Monument2 = Instantiate(Monument2, transform);
		Monument3 = Instantiate(Monument3, transform);

		Forest1 = Instantiate(Forest1, transform);
		Forest2 = Instantiate(Forest2, transform);
		Forest3 = Instantiate(Forest3, transform);

		Plains1 = Instantiate(Plains1, transform);
		Plains2 = Instantiate(Plains2, transform);
		Plains3 = Instantiate(Plains3, transform);

		Rock1 = Instantiate(Rock1, transform);
		Rock2 = Instantiate(Rock2, transform);
		Rock3 = Instantiate(Rock3, transform);

		Grass1 = Instantiate(Grass1, transform);
		Grass2 = Instantiate(Grass2, transform);
		Grass3 = Instantiate(Grass3, transform);

		StoneWall1 = Instantiate(StoneWall1, transform);
		StoneWall2 = Instantiate(StoneWall2, transform);
		StoneWall3 = Instantiate(StoneWall3, transform);

		Bush1 = Instantiate(Bush1, transform);
		Bush2 = Instantiate(Bush2, transform);
		Bush3 = Instantiate(Bush3, transform);

		HealthBush1 = Instantiate(HealthBush1, transform);
		HealthBush2 = Instantiate(HealthBush2, transform);
		HealthBush3 = Instantiate(HealthBush3, transform);

		InvigoratingBush1 = Instantiate(InvigoratingBush1, transform);
		InvigoratingBush2 = Instantiate(InvigoratingBush2, transform);
		InvigoratingBush3 = Instantiate(InvigoratingBush3, transform);

		FortifyingBush1 = Instantiate(FortifyingBush1, transform);
		FortifyingBush2 = Instantiate(FortifyingBush2, transform);
		FortifyingBush3 = Instantiate(FortifyingBush3, transform);

		ReflexBush1 = Instantiate(ReflexBush1, transform);
		ReflexBush2 = Instantiate(ReflexBush2, transform);
		ReflexBush3 = Instantiate(ReflexBush3, transform);

		QuickSand1 = Instantiate(QuickSand1, transform);
		QuickSand2 = Instantiate(QuickSand2, transform);
		QuickSand3 = Instantiate(QuickSand3, transform);

		Mountain1 = Instantiate(Mountain1, transform);
		Mountain2 = Instantiate(Mountain2, transform);
		Mountain3 = Instantiate(Mountain3, transform);

		Spike1 = Instantiate(Spike1, transform);
		Spike2 = Instantiate(Spike2, transform);
		Spike3 = Instantiate(Spike3, transform);

		PureWater1 = Instantiate(PureWater1, transform);
		PureWater2 = Instantiate(PureWater2, transform);
		PureWater3 = Instantiate(PureWater3, transform);

		Shoal1 = Instantiate(Shoal1, transform);
		Shoal2 = Instantiate(Shoal2, transform);
		Shoal3 = Instantiate(Shoal3, transform);

	}
}