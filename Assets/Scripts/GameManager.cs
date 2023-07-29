using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// You really want to have a GameManager in every single game you make. I have added one here to give you a start, but I recommend making one that is a Singleton. 
/// One of the issues I worry about is that you are creating all of your maps by hand, and doing so by placing assets. This is a lot of work, and I think is going to take an excessive amount of time. For something like a maze, where there is no real height component, you want to be able to create levels in 2D and then use some way to load this information in a 3d level. Here is an example of creating a custom level editor in unity that you could use: https://www.youtube.com/watch?v=B_Xp9pt8nRY. Look at what one of the commenters said about the GenerateTile function for 3D, as the logic is the same in 2D and 3D. Essentially you will have a palette you paint with and then you will draw over a 2d grid and then use that 2D grid to make a level.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
