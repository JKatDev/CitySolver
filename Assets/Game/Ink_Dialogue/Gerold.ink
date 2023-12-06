VAR ifDoneTalking = false
->GeroldFirstText

===GeroldFirstText===				
Hey, kid! {ifDoneTalking: ->AfterTalking|->Gerold}

===AfterTalking===
You better get your stuff all sorted, we're almost there!

->DONE 

===Gerold===
You're the one getting off at Fanville, aren't ya?											
Quite the nice town... I mean, this train has been passing there for the last 50 years, from what I've heard!	

Are you just passing by?	

+[No, I'm moving there today.]					
    Spectacular! A new home means a new life, so do enjoy it lots... eh...
    
+[Depends on how much I like it there...]		
    No need to be negative, I'm sure you'll love it there, ...um-	
    
-What's your name, fella?

//(event for player to type in name. Must be at most 15 characters. Gender options will also be available under the name input space)	

-(Name)? What a beauiful name!																									
																									
+[No! It's a cool name for a cool guy!]				
    Ah! My apologies "cool" guy.
    
+[Perfect for a lady of my caliber!] 				
    Yes, a beautiful name for a beautiful lady!													

-I haven't told you right? The name's Gerold! Remember me as one of the best conductors across the lands!																		
KEKEKEKE! I'm just joking with ya.																									
Anyways, do you already know anything about where you're heading?	

+[I heard the people there are an odd bunch.]						
    You heard right! Always somethin' with those folk. 				
    From time to time I catch wind of an interesting story from there.																								
+[I've seen photos of some places. Seems there's lots of nature there.]														
    That's right! The town is real old school, no roads or cars. Just the dirt on your paws!																						
+[Nope! What's up with the name "Fanville" anyways?]			
    Beats me. Maybe if I were a bigger FAN of that place, I'd know!
    KEKEKEKE!													
    ...														
    Sorry, that was bad, wasn't it?	
    
-Anyways, enough yappin'!																									
We're almost there, so you outta go get your stuff ready.																									
Good luck with your new life!
~ifDoneTalking = true

->DONE 