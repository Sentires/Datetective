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
    ~ Speaker (-1)
Jestar's face lights up at the slightest semblance of interest in what he does.
     ~ Speaker (1)
Hehe, well at the moment I just run any odd jobs that people want some entertainment at, usually kid's birthdays or pop up carnivals, but someday I wanna make it big and join a circus troupe! 
I do the usual comedy routine you'd expect from a clown, but I also throw in some magic tricks to bring the audience into it. The excitement on kids faces when you pull off a trick makes it worthwhile, even if it's... not the most profitable career path.
Oh! Look at me rambling- that's probably not what you meant with your question- well the plain and simple is I look for gigs on forums and Facebook groups... and I guess make connections through that.
The aquarium actually- that gig I got through a lady who had hired me for her kid's birthday party a while back. She knew someone who works here and suggested adding some interactive shows to keep kids more engaged over school break.
    ~ Speaker (0)
Heh, that's cool. I've always been more fond of freelance work. Means you can follow your own rules, and your own passions.
    ~ Speaker (1)
Y-yeah!
    -> Jestar_Conclude


=== Jestar_Aquarium ===
    ~ Speaker (0)
How'd you end up with a gig at an aquarium of all places? It's not where I'd usually expect to see a clown is all.
    ~ Speaker (1)
Ahaha yeah... a bit of a new thing they were trying out is what I was told. See, a lady who'd hired me for her kid's birthday party let me know that she was friend's with one of the managers here and that they wanted to find new ways to engage kids with the exhibits since they can get a little boring for them, and she suggested me for the job!
I had to change up my routine a little to include some more fish facts than I'm used to but I think I made it work!
Definitely not my favourite environment to work in though... There's always a bit of a looming smell- not a fishy one surprisingly, a more...sterile one? And the workers I have to interact with....
    ~ Speaker (0)
Oh? Not a fan of them?
    ~ Speaker (1)
It's- they're fine- most of them... well there's this one guy- he's always a bit of an ass for no reason, treating me like my presence is a nuisance, acting like he runs the place... and a womaniser at that! I've seen him flirting with every other woman working at Oceanarium and then acting like it never happened.
    ~ Speaker (0)
Ugh, I hate guys like that. He sounds like someone I know, what did you say his name was?
    ~ Speaker (1)
I'm... not sure... see he never even bothered to introduce himself! Acting like he's too good to even give me a name... I think I heard some other workers call him... Anthony was it? No- Antonio.
    ~ Speaker (0)
!
(The murder victim. To talk so casually about someone who just died... Is Jestar unaware, too bold for his own good, or trying to throw me off?)
    -> Jestar_Conclude
    
=== Jestar_Antonio ===
    ~ Speaker (0)
Do you know a guy who works at Oceanarium called Antonio? Figured you might have crossed paths.
    ~ Speaker (1)
Wh-why do you want to know about him? Is he talking behind my back again?!
    ~ Speaker (0)
No no, nothing like that, I just know one of the higher ups in HR- it's why I've been around the area, and... I heard he was causing some trouble. I wanted to get some first hand accounts.
    ~ Speaker (1)
Oh I'll give you some firsthand accounts...
    ~ Speaker (-1)
Wow, he believed that story without hesitation, seems like they're looking for a reason to cuss out this guy.
    ~ Speaker (1)
Such a prick! Treating me and his coworkers like our presence is a nuisance, acting like he runs the place... Oh! but only the men of course. The women? I've seen him flirting with every other woman working at Oceanarium and then acting like it never happened when they catch on that they aren't the only one he treats nicely. Heartbreaking bastard...
    ~ Speaker (0)
    Jeez, sounds like a real piece of work... I'll look into it further.
    (To talk so casually about someone who just died... Is Jestar unaware, too bold for his own good, or trying to throw me off?)
    
    -> Jestar_Conclude

=== Jestar_Conclude ===
    ~ Speaker (0)
Thanks for entertaining my questioning, and I hope you get yourself another gig soon. And let me know if I can help in any way, I'll be around the area.
    ~ Speaker (-1)
You shoot Jestar a little wink as you wave goodbye.
    ~ Speaker (1)
Y-yeah! No worrries- and thanks.
    -> END