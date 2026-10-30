//LAFAYETTE ST ---------------------------------------------------------------------------------------------------------------------

=== Jestar_Meet ===
     ~ Speaker(-1)
  You see a young man frantically looking around and repeatedly checking his pockets. He looks like an ordinary civilian at first, and perhaps he still is, but you realise they also looks similar to one of the suspects.
     ~ Character (CHAR_CLOWN, 1)
    ~ Speaker(1)
    ~ Alias(1, "Young Man")
  Crap- no no no- If I can't get back inside...
  ~ Speaker(0)
   You alright there? Looking for something?
    ~ Speaker(1)
   Ah!
    ~ Speaker(-1)
  He jolts back and flails his arms as if he's been caught doing something he wasn't meant to.
      ~ Speaker(1)
  I- um- my keys- work keys. Lost them, that's all. Not that I'll be able to go to work anytime soon...
    ~ Speaker(0)
  Oh? How come?
    ~ Speaker (-1)
  The man stared in the direction of the aquarium.
      ~ Speaker(0)
  Ah... I see. 
  Don't see why you'd need your uniform though.
    ~ Speaker (1)
  Um... it's a long story... I work there but technically I don't? A side gig of sorts- performance stuff, comedy, a-anyway I needed it back in case I can get work elsewhere.
    ~ Speaker(0)
  Mhmm... (don't see why he'd need an aquarium-specific uniform for a new job but...)
  Well, best of luck with that. But I get the feeling the case will be solved sooner than you think, you'll be back to work in no time.
  (I'll keep an eye out for the uniform just in case it leads to something more sinister with this guy...)
    -> END
    
=== Jestar_Uniform ===
    ~ Speaker(-1)
The young man you spoke to earlier is still hanging around outside the aquarium looking defeated.
    ~ Speaker (0)
Hey you, you still need that uniform?
    ~ Speaker (1)
Me- yes, yeah...
    ~ Speaker(-1)
You hand them the clown costume. He takes it with confusion at first, seemingly listening to you without actually registering what he was being handed, but once the dots connect his eyes light up immediately.
    ~ Speaker (1)
This... thank you! You're a lifesaver, really!
// QUEST - fade screen to black and then fade back in with Jestar's civilian sprite switched out for his clown costume
    ~ Speaker(-1)
What. What? How did he do that. Where did the makeup come from. What? Before you can ask any questions, they beat you to it.
    ~ Speaker (1)
Wait- where did you find it? 

* [I found it in the bin.] 
    -> Jestar_Lie
* [It was in the aquarium, duh.] 
    -> Jestar_Truth
    
    === Jestar_Lie ===
    ~ Speaker (0)
I found it in the dumpster in the alleyway behind the aquarium. Thankfully it was right on top of a tied bag, no other rubbish was touching it.
    ~ Speaker (1)
The dumpster?! How could they?! Ooooh and I just KNOW which bastard would've done it too...
Nevermind that, thank you again for finding it for me! Now the the Jestar can shine in the night sky again!
And maybe I can find a new gig....

    ~ Speaker (0)
Oh yeah, I wanted to ask about that.

* [How does the whole clown thing work?] 
    -> Jestar_Work
    * [How'd you get a gig in an aquarium?] 
    -> Jestar_Aquarium
* [Do you know Antonio?] 
    -> Jestar_Antonio
    
    
    === Jestar_Truth ===
    ~ Speaker (0)
It was in the aquarium like you said it would be.
    ~ Speaker (1)
...?
    ~ Speaker (0)
...!
    ~ Speaker (-1)
WHY DID YOU SAY THAT? So much for being discrete about your position here. You try to act like nothing you said was suspicious but they definitely caught on.
        ~ Speaker (1)
How did you get inside...? It's meant to be closed for repairs.
    ~ Speaker (0)
...A key, just like what you were looking for to get in. I was asked to get some supplies from the back by a friend who works there.
    ~ Speaker (-1)
You watch Jestar squirm as you call him out on his hypocrisy, nice save. With a playful smirk, you prod them further while their guard is down.
* [How does the whole clown thing work?] 
    -> Jestar_Work
    * [How'd you get a gig in an aquarium?] 
    -> Jestar_Aquarium
* [Do you know Antonio?] 
    -> Jestar_Antonio


    
=== Jestar_Work ===
     ~ Speaker (0)
How does this whole clown thing work? I think it sounds fun.




    -> END

=== Jestar_Aquarium ===
placeholder
    -> END
    
=== Jestar_Antonio ===
placeholder
-> END

