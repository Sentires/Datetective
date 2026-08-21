A new case file was on your desk this morning.
Another crime the police force couldn't solve themselves. Typical.
This wasn't the first time they turned to your humble little detective firm for help, you doubt it'll be the last either, but hey, you were never one to turn down an interesting case.
-> Office

=== Office ===
 + [Interact with case file] -> Case_file
 +[Interact with PC] -> PC
 +[Interact with corkboard] -> Corkboard
 +[Interact with filing cabinet] -> Filing_cabinet
 +[Interact with bin (before PC) ] -> Bin1
 +[Interact with bin (after PC) ] -> Bin2
 === Case_file ===
 A standard case file with your detective agency's emblem printed on the front. Inside you find small silver key, and three suspect profiles, with photographs attached. 
 A fourth sheet of paper slips out.
 'Further information can be found by logging into the database.'
 Juno: *sigh*, well at least they entertain my old-timey aesthetic just a little bit.
-> Office 
 
 === PC ===
A standard desktop computer, one you bought purely because your manager kept pestering you to get over the 'classic' detective experience and get with the times. It does it's job well enough, but you still think paper files and corkboards with evidence tied together with red string are way cooler. 
A shame no one is willing to comply with your methods, at least not fully.
Juno: ...
Juno: .....
Juno: ..What was the password again..?
You vaguely remember receiving a file with the login information back when you first began engaging with the local police.
...and promptly throwing it out after confidently thinking you could remember it all.
 -> Office
 
 === Corkboard ===
A corkboard covered in photographs and notes from your last major case. Everything is connected with a piece of red string, creating a mesmerising web. 
-> Office
 
 === Filing_cabinet ===
 You open the cabinet to rows of neatly organised files documenting your past cases.
 
 They're ordered by date. 
 
 You used to organise them alphabetically and give them elaborate names, but your manager kept complaining that it was a nightmare to remember all your 'wannabe mystery novel' titles.
  -> Office
  
 === Bin1  ===
 A simple mesh bin filled with paper scraps. 
 ...You cannot remember the last time you emptied it out.
  -> Office
 
  === Bin2  ===
If you're lucky, that old file with the password on it might still be in here...
  -> Minigame1
  
  === Minigame1 ===
  [PAPER PUZZLE MINIGAME HERE]
  -> Office_PostPuzzle
  
  === Office_PostPuzzle ===
   +[Interact with PC] -> PC2
   + [Leave Apartment] -> Leave
 === PC2 ===
 You enter the password. 
 
 [Graphic with case information]
 
 'Despite all the suspects being interrogated by police, none were willing to give substantial information, and the interogators were unable to find justifiable reasoning to keep them in police holding.
 
 Police have reason to believe that all three suspects possess knowledge that could assist in this case, however their lack of cooperation has stagnated progress.
 
  Locations of interest:
 
 - Aquarium - 63 Lafeyette Street (Crime Scene)
 - Baked Hearts Patisserie & Cafe - 64 Lafeyette Street'
 
 The Aquarium has been closed off from the public while the investigation takes place. Please enter via the backdoor entrance accessible through the alleyway next to Baked Hearts Patisserie & Cafe. 
 
 Juno: Huh, Lafayette isn't too far from here. Guess I can get right to work.
  -> Office_PostPuzzle
  
=== Leave ==
[DRIVING CUTSCENE GRAPHIC HERE]
+ [Lafayette Street] -> Lafayette_Street

=== Lafayette_Street ===
A bustling street in Lower Manhatten. Despite now being the scene of a crime, people walking about seem to be going on about their business without any concern.
 +[Interact with young man frantically searching his pockets] -> Jestar
 +[Interact with elderly woman staring at the aquarium] -> Elder
 +[Enter the alleyway] -> Alleyway
 
 
 
 === Jestar ===
  You see a young man frantically looking around and repeatedly checking his pockets.
  Young Man: Crap- no no no- If I can't get back inside...
  Juno: You alright there? Looking for something?
  Young Man: Ah!
  He jolted back and flailed his arms as if he had been caught doing something he wasn't meant to.
  Young Man: I- um- my keys- work keys. Lost them, that's all. Not that I'll be able to go to work anytime soon...
  Juno: Oh? How come?
  The man stared in the direction of the aquarium.
  Juno: Ah... I see. 
  Juno: Don't see why you'd need your uniform though.
  Young Man: Um... it's a long story... I work there but technically I don't? A side gig of sorts- performance stuff, a-anyway I needed it back in case I can get work elsewhere.
  Juno: Mhmm... (don't see why he'd need an aquarium-specific uniform for a new job but...)
  Juno: Well, best of luck with that. But I get the feeling the case will be solved sooner than you think, you'll be back to work in no time.
  Juno: (I'll keep an eye out for the uniform just in case it leads to something more sinister with this guy...)
  
  -> Lafayette_Street
  
  === Elder ===
  You see an older lady staring wistfully at the closed off aquarium.
  Elderly Woman: Terrible... just terrible...
  She seems to be lost in thought, speaking to herself. Best not to disturb her.
   -> Lafayette_Street
   
   === Alleyway ===
   +[Bakery backdoor] -> BakeryBackdoor
   +[Aquarium backdoor] -> AquariumBackdoor
   
   === BakeryBackdoor ===
   This isn't the scene of the crime, though still an area of interest.
   +[Leave] -> Alleyway
   +[Try opening the door] ->BakeryOpenAttempt
   
   === BakeryOpenAttempt ===
   You attempt to turn the door knob. Unsurprisingly, it doesn't open.
   
   Since you have nothing better to do, you try inserting the aquarium key into the lock. It goes in all the way until the second last notch, but doesn't open.
   Juno: (Hmm... same locksmith for both buildings.)
   -> Alleyway
   
   === AquariumBackdoor ===
   As you approach the backdoor, covered in police tape like the front of the aquarium, you make sure to double check that you still have the case file and your detective badge on your person before entering. 
   click!
   
   The door unlocks seemlessly with the key you were given.
   
    +[Enter Aquarium] -> Storeroom
    
    === Storeroom ===
    It's pitch black inside. You feel along the walls and find yourself coming into contact with shelves and cardboard boxes. A storage room of sorts, it seems.
    [MAZE MINIGAME HERE]
    
    +[Crime scene] ->CrimeScene
    
    === CrimeScene ===
    
    +[Investigate the body] -> Body
    +[investigate stray blood stain] -> Blood
    +[Investigate fish tank] -> Tank
    
    === Body ===
    The lifeless body lays before you.
    [Cutscene art close up]
    The victim was male, 28, middle class background, worked at the aquarium as an educator doing tour guides for families.
    
    Cause of death (uhhh figure this out)
    
    ->CrimeScene
    
    === Blood ===
    text
    -> CrimeScene
    
    === Tank ===
    text
    -> CrimeScene
    
   
   
END SCENE
    -> END
