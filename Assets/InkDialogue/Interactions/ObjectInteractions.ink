//OFFICE ------------------------------------------------------------------------------------------------------------------------------

=== Case_file ===
     ~ Speaker(-1)
 A standard case file with your detective agency's emblem printed on the front. Inside you find small silver key, and three suspect profiles, with photographs attached. 
 //JACK PLEASE INSERT - Case file suspect illustrations x3 appear on screen
 A fourth sheet of paper slips out.
 'Further information can be found by logging into the database.'
 ~ Character(CHAR_DETECTIVE, 0)
 ~ Speaker(0)
 *sigh*, well at least they entertain my old-timey aesthetic just a little bit.
    -> END
    
    === PC_Locked ===
     ~ Speaker(-1)
A standard desktop computer, one you bought purely because your manager kept pestering you to get over the 'classic' detective experience and get with the times. It does it's job well enough, but you still think paper files and corkboards with evidence tied together with red string are way cooler. 
A shame no one is willing to comply with your methods, at least not fully.
  ~ Speaker(0)
...
.....
..What was the password again..?
    ~ Speaker(-1)
You vaguely remember receiving a file with the login information back when you first began engaging with the local police.
...and promptly putting it in your paper shredder after confidently thinking you could remember all the information on it.
    -> END
    
=== PC_Unlocked ===
    ~ Speaker(-1)
 You enter the password. 
 
 //JACK PLEASE INSERT - Case file crime log illustration appears on screen
 
 'Despite all the suspects being interrogated by police, none were willing to give substantial information, and the interogators were unable to find justifiable reasoning to keep them in police holding.'
 
 Police have reason to believe that all three suspects possess knowledge that could assist in this case, however their lack of cooperation has stagnated progress.
 
  Locations of interest:
 
 - Aquarium - 63 Lafeyette Street (Crime Scene)
 - Baked Hearts Patisserie & Cafe - 64 Lafeyette Street'
 
 The Aquarium has been closed off from the public while the investigation takes place. Please enter via the backdoor entrance accessible through the alleyway next to Baked Hearts Patisserie. 
 
   ~ Speaker(0)
 Huh, Lafayette isn't too far from here. Guess I can get right to work.    
    -> END
    
=== Corkboard_Start ===
    ~ Speaker(-1)
Your trusty corkboard looms before you, waiting for evidence to cover it's surface and be strung together with red string because that's like, a super cool visual.
    -> END

//QUEST TRIGGER - Office scene is changed at the end of each route to include evidence minigame
=== Corkboard_End ===
    ~ Speaker(-1)
Finally, it's time to fulfil your classic detective fantasy and connect all the evidence with red string despite it being much more efficient to document it online like the Ondarion Police keep asking you to.  
// WIRES PUZZLE ACTIVATE
    -> END
    
=== Filing_cabinet ===
    ~ Speaker(-1)
You open the cabinet to rows of neatly organised files documenting your past cases.
They're ordered by date. 
You used to organise them alphabetically and give them elaborate names, but your manager kept complaining that it was a nightmare to remember all your 'wannabe mystery novel' titles.
    -> END
    
=== Bin_No_Files  ===
    ~ Speaker(-1)
A simple mesh bin filled with paper scraps...You cannot remember the last time you emptied it out.
    -> END
    
// QUEST? No changes in the office assets but different dialogue is triggered once player has interacted with === case_file === 

=== Bin_After_Files  ===
     ~ Speaker(-1)
If you're lucky, that old file with the password on it might still be in here...
    -> END
// SLIDING PUZZLE ACTIVATE


//LAFAYETTE ST ---------------------------------------------------------------------------------------------------------------------

=== Lafayette_Enter ===
//Dialogue that automatically plays when entering the scene if possible
    ~ Speaker(-1)
You arrive on Lafayette Street. It's much quieter than it has been when you have visited in the past. Notably, the usual bustle near the aquarium is missing.
    -> END
    
=== Bakery_Front ===
    ~ Speaker(-1)
A small bakery fills the streets with the entrancing smell of warm baked goods. 'Baked Hearts Patisserie' is scrawled across the front of the store. 
The building seems to be connected to the aquarium next door. An odd collaboration you would think at first, but it would make sense that visitors of the latter would like a place to rest and grab a bite after lots of walking.
    -> END

=== Aquarium_Front ===
    ~ Speaker(-1)
The aquarium stands tall and proud. Usually there would be a bustling queue going out the door and down the street on a sunny day like this, but the crime that has happened within its walls has left the whole building locked up. 
There's a lack of caution tape or cop cars surrounding the premise. It seems that the police are trying to keep the murder under wraps to avoid public panic until more evidence comes out. A small note on the aquarium's door reads "CLOSED FOR REFURBISHMENTS".
    -> END
    
    
//AQUARIUM -------------------------------------------------------------------------------------------------------------------------

=== Fish_Tank ===
    ~ Speaker(-1)
A massive fish tank looms over you. Inside are varying forms of aquatic life, from colourful schools of fish to turtles. They mindlessly swish back and forth in their enclosure, unaware of, or perhaps not bothered by the lifeless body splayed before them.
    -> END

=== Caution_Tape ===
    ~ Speaker(-1)
Yellow and black caution tape outlines the lifeless body, a precaution you won't be paying any mind to.
    -> END
=== Corpse ===
    ~ Speaker(-1)
The lifeless body of a young man lays before you. Deep crimson stains his plain collared shirt, and a pool of blood wets the tile below him. The killing blow, well, blows, appear to be an array of haphazard stab wounds aimed at the heart. Not all the stabs hit their target, this appears to be the work of an amateur. 
    -> END
    
=== Clown_Costume ===
    ~ Speaker(-1)
You notice a colourful bundle of clothes folded up behind the stage. Unfolding it, you realise it is a clown costume. What in the world is a costume like that doing here..? As you question the costume's presence, you remember the frantic young man you met outside the aquarium.
    ~Speaker (0)
Ooohhhh! 'performance, comedy' okay yeah that makes a bit more sense.
Wait no it doesn't. What kind of aquarium has a clown?? 
    ~ Speaker(-1)
You fold up the costume again and take it with you. Returning it to it's presumed owner might give you an opening to ask some questions.
    -> END
    
/*BAKERY*/

=== Bakery_Table ===
    ~ Speaker(-1)
A simple but cute table setup. There are napkins, spare cutlery, and a copy of the menu tucked into a small organiser resting on it's wooden surface.
Upon closer inspection of the wood, you can see telltale signs of use. Namely, small etchings absentmindedly made by customers past, and faint coffee stains that seeped too far into the grain to clean out.
    -> END
    
=== Bakery_Cabinet ===
    ~ Speaker(-1)
Right next to the counter, as if to lure you in to make a last minute additional purchase, sits a display cabinet with a wide variety of cutely decorated pastries. They look fresh and made with love.
    -> END
    
//QUEST TRIGGER - Lost and found box, Briar behind counter
=== Bakery_Box_Briar ===
    ~ Speaker(-1)
A small pink box sits on the side of the counter with a handwritten sign taped to the front reading 'Lost & Found'. Inside are the typical items you would expect people to absentmindedly leave behind, a hair bow, a kid's toy car, some lip gloss to name a few. It's a nice gesture from the establishment to hold onto them.
Upon closer inspection however, you notice a much more valuable item at the bottom of the box. A smartphone, a recent model as well. Trying not to draw attention to yourself, you sift through the box and push the other objects out of the way so you can turn it on.
It lights up, revealing several unread messages from a contact named 'Antonio <3'.

  ~ Speaker(0)
..! That's the victim's name. It could be a coincidence, but it'd be worth double checking anyway. If only that baker wasn't so close by...
    -> END
    
/*Lost and found box, unattended, investigated prior*/
Seemes like Briar finally went on a lunch break or something. You quickly fish out the phone from the lost and found box and switch it on to read the text message previews.
2:57am - 'Hey baby, got us those tickets for that movie I...' 
the text fades out, you'd need to unlock the phone to see the rest of it. Luckily for you, he had sent a follow up message.
2:59am - Use the back door so she doesn't see you, I left it unloc...'


/*Lost and found box, unattended, NOT investigated prior*/
A small pink box sits on the side of the counter with a handwritten sign taped to the front reading 'Lost & Found'. Inside are the typical items you would expect people to absentmindedly leave behind, a hair bow, a kid's toy car, some lip gloss to name a few. It's a nice gesture from the establishment to hold onto them.
Upon closer inspection however, you notice a much more valuable item at the bottom of the box. A smartphone, a recent model as well. Trying not to draw attention to yourself, you sift through the box and push the other objects out of the way so you can turn it on.
It lights up, revealing several unread messages from a contact named 'Antonio <3'. 
/*Juno*/ ..! That's the victim's name. It could be a coincidence, but...
You fish out the phone and read what you can of the text message previews.

2:57am - 'Hey baby, got us those tickets for that movie I...' 
the text fades out, you'd need to unlock the phone to see the rest of it. Luckily for you, he had sent a follow up message.
2:59am - Use the back door so she doesn't see you, I left it unloc...'


/*ALLEYWAY*/


/*ANDRONE ST*/