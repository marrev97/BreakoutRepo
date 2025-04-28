Game was developed in Unity 2022.3.61f1

Load the scene "Main" in /Assets

Custom levels are placed in Assets/Resources/CustomLevels.

It must be named "levels" for the game to read it.
The format is as follows 

L<N1> C<N2> R<N3> <N4>   

N1 is the level, n2 is number of columns, N3 is number of columns, n4 is the probability of a brick turning into a power up. 
(Max of 2 per level)>

example:

l1 C12 R2 1010      // level 1, 12 columns, 2 rows,  10% probability of a brick becoming a power up
l2 C3 R3 11110      // level 2, 3 columns, 3 rows, 30% probability of a brick becoming a power up


The formatNumber solution was placed in ReadInLevelsLoader.cs
and in the function 
    public static int FormatNumber(int number)
    {
        return Convert.ToInt32(number.ToString(), 2);
    }
