//INCLUDE Cat.ink
VAR ifAcceptedQuest = false

->DinoFirstText

===DinoFirstText===

Hello there! {ifAcceptedQuest: ->Accepted|->Dino} #speaker:Dino #portrait:dino_neutral
->Dino

===Accepted===
It works ahhahahafhfd #portrait:dino_happy

->DONE 

===Dino===
Welcome to my shop! #speaker:Dino #portrait:dino_neutral


I haven't see you before, you must be a new face!


+[Yup, just moved in!]
    I had a feeling! #portrait:dino_happy
+[Is it that obvious?]
    Yup, never seen anyone one like you around here. #portrait:dino_neutral
    
-But it's always nice to meet new people.

+[House party?] Come to your house warming party?
+[Party at my place!] Oh, a party!?

-Oh I would love to, but sadly my shipment of veggies is late and i can't leave my post until i recive them.... #portrait:dino_sad

+[I can help!]
    You would bring them for me? Fantastic! They should be at the post office! #portrait:dino_happy
     ~ifAcceptedQuest = true
    ->DONE

+[Sucks to be you.]
    ... #portrait:dino_sad
    ->DONE
    

    
