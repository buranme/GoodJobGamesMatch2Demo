Mehmet Mert Buran
Demo of a Match2 game made for Good Job Games

Remarks:
* The main class that controls the game flow is Board
* Helper class has been created to help Board with calculations

* BlockAnimator iterates through each Block that is supposed to be moving on each FixedUpdate
and moves them slightly towards their destination until they reach it
* While there are moving Blocks the player input is disabled
* BlockAnimator notifies the Board when it is done animation for the round so that the Board
can check the new alignment of Blocks and enable input again
* The basic game flow follows the communication of the Board and the BlockAnimator

* Since the board needs to shuffle when there is no possible block to click on,
when the animation ends, the game calculates which Blocks each Block destroy if clicked on
* According to the amount of Blocks each Block would destroy, they all change their images
so that they have the plain, rocket, bomb or swirl icon

* A Scriptable Object is used to store the necessary variables, if you want to change a value
you need to do it on the object called "LookupScriptableObject" under Assets/ScriptableObjects

* A Block Pool is used to push-pull Blocks instead of Instantiate-Destroy them to save performance

* The special blocks of Rocket, Bomb and DiscoBall have also been added, these don't use the Pool
* The Rocket, Bomb and DiscoBall, along with StandardBlock which can have colors and form chunks
are children of the parent class Block

* When the Helper calls each Block to calculate the explosions, each children does it in their own way
* StandardBlock uses the Flood Fill algorithm to find neighboring Blocks with same color
* Rocket gets the Blocks in the same column/row depending on if the Rocket is vertical or horizontal
* Bomb gets the 8 blocks that surround it
* DiscoBall gets all the StandardBlocks in the Board that it shares its color with