# Managed Doom

![Managed Doom](screenshots/window.png)

The goal of this project is to make a fully functional Doom source port written in C# without unsafe code.  

The rendering engine is almost complete and capable of showing 3D view like the screenshots below.  

Now I'm working on the gaming code. Although several functionalities are still missing, most weapons, monsters and gimmicks work much like the original game.  



## Screenshots

![E1M1 start](screenshots/doom-e1m1-start.png)

![E1M1 outside](screenshots/doom-e1m1-outside.png)

![A fake 3D bridge in Requiem MAP13](screenshots/requiem-map13-bridge1.png)

![Another 3D bridge](screenshots/requiem-map13-bridge2.png)

![Slime trail](screenshots/doom-e1m1-slime.png)



## Video sample (Doom 2, demo 3)

https://www.youtube.com/watch?v=LZpyeLlm_1Y  

[![YouTube video of Managed Doom](https://img.youtube.com/vi/LZpyeLlm_1Y/0.jpg)](https://youtu.be/LZpyeLlm_1Y)



## Todo

- __Software renderer__  
    * [x] Front-to-back rendering using BSP
    * [x] Wall texture mapping
    * [x] Ceiling / floor texture mapping
    * [x] Transparent textures
    * [x] Diminishing lighting
    * [x] Sky rendering
    * [x] Sprite rendering
    * [x] High resolution rendering
    * [x] Optimization

- __Gaming code__
    * [x] Collision detection
    * [x] Player movement
    * [ ] Player Attack
    * [x] Monster AI
    * [ ] Doors and platforms
    * [ ] Animated textures
    * [x] Demo compatibility (All the v1.9 IWAD demos can be played)

- __Audio__
    * [*] Sounds
    * [ ] Music



## References

- __The original source code by id Software__  
https://github.com/id-Software/DOOM

- __The Game Engine Black Book: DOOM by Fabien Sanglard__  
If you want to understand the big picture of the rendering process in Doom, buy this one!  
http://fabiensanglard.net/gebbdoom/

- __The Unofficial Doom Specs by Matthew S Fell__  
http://www.gamers.org/dhs/helpdocs/dmsp1666.html

- __Doom Wiki__  
https://doomwiki.org/wiki/Entryway

- __SFML - Simple and Fast Multimedia Library__  
https://www.sfml-dev.org
