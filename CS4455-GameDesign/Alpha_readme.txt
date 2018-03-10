CS4455-GameDesign
Alpha Demo Fall 2017
Copyright (c) 2017, Ctrl+Alt+Delicious (Ryan Berst, Jessica Moranville, Matthew Nasiatka, Jake Williams, Han Zhang)

Built on Windows. 

Our game currently does not have a clear set of instructions presented to the player at the start. This will be done through multiple npc’s you communicate with. Because of this, we will explain the goal here: You start in the center of randomly generated maze that contains 5 puzzle rooms and enemies. The enemies chase you through the walls. You can break through walls every few seconds with the “B” button on a joystick. Each puzzle room has its own set of rules. In one there are flocking blocks and you must touch the different one. In two you must avoid enemies and collect glowing orbs and in the fourth you must slide blocks to block a laser and push another block onto a control pad. After this you will be presented with a room where you must move 9 blocks to complete a pattern. The player can pick up different colored markers and drop them in significant locations. Completing a puzzle room returns the player to the center of the maze. The pause menu can be reached by pressing Start button. 


• Our game is a 3D feel game with success and fail conditions. In our current alpha we track health and completion of puzzles but do not have a transition to a fail or success state.
• At any point in the game, the player can reach the pause menu and exit the game. This returns the player to the start menu, where he or she can start the game.
• The perspective is third-person.
• As the player is pursued by enemies in the maze, they can choose to break a wall. This ability has an associated cooldown, forcing the player to choose between running along the maze, or breaking a wall.
• The player can interact with elements within the game world, such as ledges, NPCs, and other objects.
• The camera follows behind the player at a reasonable distance. The camera can clip through walls to avoid being obstructed.
• Audio cues help the player find objectives in the maze.
• There is no noticeable unintended clipping of objects. Note that there are enemies in the game that intentionally have the ability to phase through walls.
• Physics simulation is consistent throughout the game. 
• AI enemies are reasonably balanced. They move at speeds that are comparable to the player’s speed, build tension without introducing frustration.
• Player controls a mecanim-controlled, blendtree-enabled character. 
• Aside from moving between puzzle rooms, player has sole control over the character. 
• The puzzle room with flocking cubes currently allows for infinite falling off the edge, but it is a known issue and is the only location within the game where this is possible. 
• There are pressure plates in one puzzle room (with the laser) that open a secret door, non-player mecanim-controlled objects in another puzzle room (with the giant zombunny) that also blow up upon collision with the player and cause player to lose health. 





