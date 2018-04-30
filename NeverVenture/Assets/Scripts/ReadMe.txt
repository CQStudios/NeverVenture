PlayerBehavoiur:
	Created an Inventory System:
	-Stash: The Player's Cache of items that the ycan choose to equip. This holds all of the upgrade info for each item
	-Equipment: List of the items thet the player has equipped and the enchantment chosen for that item.
		-Slot 0: Brawn effected item
		-Slot 1: Agility effected item
		-Slot 2: Vitallity effected item
		-Slot 3: Intelligence effected item
		-Slot 4: Wisdom effected item
		-Slot 5: Willpower effected item
		-Slot 9: Fits in any Slot, used only by Nothing Equipped
	To Equip an item

	PlayerBehaviour.equipItem(int ID, int enchant)       // Use ID 0 to Equip Nothing (Uses Slot 9)
		- This will equip an item into the proper slot using the slot codes on the base item from the Database.

Item Database:
		Contains all base items in the game (24) in a C# object
		Constructor
		ItemDatabase()    //Initalizes all items with database values
		Functions
		ItemDatabase.getItem(int ID)
			Returns the Database Item by ID

Item:
	-Format is ready for an XML Database
	-Each Variable is commented to explain the purpose of the information stored in the Item Database
	-Bottom has every Damage Type in NeverVenture with the code number next to it
		-Basic Damages 1, 2, 3, 7, 8, 9
		-Enchant Damages 4, 5, 6, 10, 11, 12
		-Armor Resistances 13, 14, 15, 16

FullItem:
	Uses Database Items and PlayerBehaviour's Stash values to dynamiclly create an exponental number of items in-game.
	
	Constructors
	FullItem(int ID)    // Creates a blank new FullItem for the Stash. Used only for New Character Creation
	FullItem(int ID, int tier, int enchant)   //Creates an FullItem for the player to equip
		NOTE: Might not use the porper enchantment tier, might need debugging

PlayerData:
	Saves the Stash and Equipment