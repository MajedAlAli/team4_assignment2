# Team4 Assignment 2

## Main Contributions

- **Majed**: Downloaded and completed the challenge, created and implemented six different powerups and create the mainmenu.  
- **Mohammad**: upgraded the overall aesthetics(look) and feel of the game, implemented the score system and the timer, created the sound design of the game.
- **Ebrahim**: *[Pending]*  

## Branch Structure

1. The **main** branch was used to complete the initial challenge.  
2. Three branches were then created from **main**:  
   - **Branch1**: Focused on aesthetics, sound design, game design, and the scoring system.  
   - **enemy-improvement**: Developed different types of enemies.  
   - **powerup**: Developed various powerups and the main menu.  

## Merging Process

Once all branches were ready for integration, we encountered a merge issue:

![Merge Issue](https://github.com/user-attachments/assets/823fae76-5e80-492e-a4bc-964ef203f850)

To resolve this, we merged the branches manually in the following sequential order:

1. **Branch1** and **powerup** were merged into **Branch1_powerup**.  
2. **enemy-improvement** was then merged with **Branch1_powerup** into the final **All_Branches** branch.
3. **All_Branches** was corrupted, so we branched off it from a stable commint to **Main_Branch** branch.

## Notes
- Due to the history differences between branches, we had to manually resolve conflicts and ensure a smooth integration.  
- Future improvements should focus on maintaining a consistent branching strategy to avoid similar issues.


