# Lab Submission 10
 
 The best way to view this program working is to have the scene view and game view both open at the same time!

 How does A* pathfinding calculate and prioritize paths?
    A* pathfinding calculates and prioritizes paths by assigning specific values to each cell. Each cell typically has a g and an h value, which are the distance to the start node and the estimated distance to the end node, respectively. These values are added up to give the call an f value, which is its total cost. The lower the cost, the 'better' the node appears to the algorithm.

What challenges arise when dynamically updating obstacles in real-time?
    Dynamically updating obstacles creates the problem of blocking an existing path. When dynamically updating obstacles it is essential to attempt to find a new path every time in case a new obstacle blocks the current path.

How could you adapt this code for larger grids or open-world settings?
    This code could be expanded upon for larger grids or open-world settings by dividing the grid up into smaller portions. Pathfinding on a large scale could be very performance heavy and cause issues. Dividing the pathfinding up into chunks allows the program to focus and complete smaller sections without overloading the system.

What would your approach be if you were to add weighted cells?
    My approach to add weighted cells would be to give them a default cost that is added to the cells f cost. This would make the cell less desireable by increasing its value while still allowing the cell to be traveled if the algorithm must choose it.

Video showcasing the program-> https://youtu.be/4pfGGnDpQDk
