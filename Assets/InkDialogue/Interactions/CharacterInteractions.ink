//LAFAYETTE ST ---------------------------------------------------------------------------------------------------------------------

=== Jestar_Meet ===
~ Character(CHAR_CLOWN, SPEAKER_SECONDARY)
     ~ Speaker(-2)
  You see a young man frantically looking around and repeatedly checking his pockets. He looks like an ordinary civilian at first, and perhaps he still is, but you realise they also looks similar to one of the suspects.
    ~ Speaker(1)
    ~ Alias(1, "Young Man")
  Crap- no no no- If I can't get back inside...
  ~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
  ~ Speaker(0)
  ~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
   You alright there? Looking for something?
    ~ Speaker(1)
   Ah!
    ~ Speaker(-2)
  He jolts back and flails his arms as if he's been caught doing something he wasn't meant to.
      ~ Speaker(1)
  I- um- my keys- work keys. Lost them, that's all. Not that I'll be able to go to work anytime soon...
    ~ Speaker(0)
  ~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
  Oh? How come?
    ~ Speaker (-2)
  The man stared in the direction of the aquarium.
    ~ Speaker(0)
    ~ Portrait(SPEAKER_PRIMARY, MOOD_SAD)
  Ah... I see. 
    ~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
  Don't see why you'd need your uniform though.
    ~ Speaker (1)
  Um... it's a long story... I work there but technically I don't? A side gig of sorts- performance stuff, comedy, a-anyway I needed it back in case I can get work elsewhere.
    ~ Speaker(0)
    ~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
  Mhmm... (don't see why he'd need an aquarium-specific uniform for a new job but...)
      ~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
  Well, best of luck with that. But I get the feeling the case will be solved sooner than you think, you'll be back to work in no time.
  (I'll keep an eye out for the uniform just in case it leads to something more sinister with this guy...)
    -> END
    
=== Jestar_Uniform ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_CLOWN, SPEAKER_SECONDARY)
    ~ Speaker(-1)
The young man you spoke to earlier is still hanging around outside the aquarium looking defeated.
    ~ Speaker (0)
    ~ Alias(1, "Young Man")
    ~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
Hey you, you still need that uniform?
    ~ Speaker (1)
Me- yes, yeah...
    ~ Speaker(-1)
You hand them the clown costume. He takes it with confusion at first, seemingly listening to you without actually registering what he was being handed, but once the dots connect his eyes light up immediately.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
This... thank you! You're a lifesaver, really!
// QUEST - fade screen to black and then fade back in with Jestar's civilian sprite switched out for his clown costume
    ~ Alias(1, "")
Jestar is back in business!
    ~ Speaker(-1)
    ~ Portrait(SPEAKER_PRIMARY, MOOD_SHOCK)
What. What? How did he do that. Where did the makeup come from. What? Before you can ask any questions, they beat you to it.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
Wait- where did you find it? 

* [I found it in the bin.] 
    -> Jestar_Lie
* [It was in the aquarium, duh.] 
    -> Jestar_Truth
    
    === Jestar_Lie ===
    ~ Speaker (0)
    ~ Portrait(SPEAKER_PRIMARY, MOOD_SAD)
I found it in the dumpster in the alleyway behind the aquarium. Terrible that someone would throw it out just like that. Thankfully it was right on top of a tied bag, no other rubbish was touching it.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
The dumpster?! How could they?! Ooooh and I just KNOW which bastard would've done it too...
Nevermind that, thank you again for finding it for me! Now the the Jestar can shine in the night sky again!
And maybe I can find a new gig....

    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
Oh yeah, I wanted to ask about that.

-> Jestar_Ask
    
    
    === Jestar_Truth ===
    ~ Speaker (0)
It was in the aquarium like you said it would be.
    ~ Speaker (1)
...?
    ~ Speaker (0)
...!
    ~ Speaker (-1)
    ~ Portrait(SPEAKER_PRIMARY, MOOD_SHOCK)

WHY DID YOU SAY THAT? So much for being discrete about your position here. You try to act like nothing you said was suspicious but they definitely caught on.
        ~ Speaker (1)
How did you get inside...? It's meant to be closed for repairs.
    ~ Speaker (0)
...A key, just like what you were looking for to get in. I was asked to get some supplies from the back by a friend who works there.
    ~ Speaker (-1)
You watch Jestar squirm as you call him out on his hypocrisy, nice save. With a playful smirk, you prod them further while their guard is down.
-> Jestar_Ask

// I made this repeatable
=== Jestar_Ask ===
  ~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
  ~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
  ~ Character(CHAR_CLOWN, SPEAKER_SECONDARY)
  ~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)
  ~ Speaker(0)
  So...
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
    
    
//ANDRONE ST ------------------------------------------------------------------------------------------------------------------------

=== Dakota_Meet ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_LANDLORD, SPEAKER_SECONDARY)
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    ...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    ...
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Dakota.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Juno.
    ~ Speaker (-1)
You haven't spoken with Dakota in three years. Not since that messy fight you'd had over him quitting the force during a case you two were leading. You both know it wasn't really about the case, but neither of you are going to admit there were more personal feelings involved.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    I'm assuming you didn't just come here to say hi. You still running errands for those cops?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Tch, they're the ones who come running to me for help now. Not that it matters.
    I'm not gonna beat around the bush, you know my methods already. You're a suspect, answer my questions and we won't have any trouble.
    
    -> Dakota_Ask // moved to here
    
// Loops once you meet Dakota
=== Dakota_Ask ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_LANDLORD, SPEAKER_SECONDARY)
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Oh and aren't you so giddy to get up and personal in my business again.
     ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_FLUSTER)
    I WAS NOT- shut up- 
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    always trying to distract me...I'm here on business.
     ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_FLUSTER)
    Heh, yeah yeah. Shoot.
    
    * [What have you been doing since you quit?] 
    -> Dakota_Work
    * [Working at a yacht club, really?] 
    -> Dakota_YachtClub
    * [Do you know Antonio?] 
    -> Dakota_Antonio
    
=== Dakota_Work ===
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Oh, you care about that now?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    No. Answer the question.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    I'm a landlord of sorts. Own most of the buildings along this street and Lafayette. Lease them out. Really scored with securing the rights to that aquarium, though it's a bit of a hassle dealing with it being directly tied to the bakery next door.
    ~ Speaker (-1)
    He's sprinkling in areas of interest without any prodding. He definitely knows something is up. Is he giving you more evidence right off the bat to protect himself from suspicion..? But it kind of loops back around to making him more suspicious...
    Still, you might not be on good terms now but you've known Dakota for years, what reason would he have to murder someone? Murder them in a building he owns as well...
    But then again, it's been three years, people can change. You both changed during your time together, it would inevitably change while you were apart... right...
     ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    Juno? Earth to Junoooo? You okay?
      ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Wuh-
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    I'm fine. I was just analysing what you said or whatever. You're not off the hook yet, but what you said has given me a good lead so... thanks.
    ~ Speaker (-1)
    You leave before you can bring yourself to look at Dakota's expression. Now's not the time to be getting sentimenal over what could have been.
        -> END

=== Dakota_YachtClub ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Working at a yacht club? Really?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    What's that supposed to mean?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    I thought you hated stuck up people, and I'm preeetty sure yacht clubs are for stuck up people.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    As much as I'd love to make this a 'gotcha' moment and say people change, I'd like to believe I haven't changed for the worse. I don't work here, I lease out the building though. I was checking in on overdue rent.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    Ewwww you're a landlord now? I'd say that's even worse.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Well at least I'm earning a good living. I've heard that you can't say the same.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    Huh?! Who the hell is saying that? AND I'M DOING JUST FINE THANK YOU. 
    ~ Portrait (SPEAKER_PRIMARY, MOOD_FLUSTER)
    No but seriously, who's saying that...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    You aren't the only one who stays in touch with the force, though I guess you're on better terms with them.
    It was Keith, he loves to get up in others' business. I'm sure he's told you things about me too.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    God, yeah. Hate that guy. Well just so you know I make more than double what he does now that I work alone.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Heh, I'll take your word for it. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Anyway, we weren't here to complain about Keith, and I've still got leases to check up on, so if you'll excuse m-
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    I never said questioning was over, did I? What was that about more leases? You own more property?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Tch. Yeah, yeah I do. Most of the buildings on this street and Lafayette are mine. That includes that aquarium I know you're investigating. And yes that was Keith talking behind your back again. Happy?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    ...Happy.
    
    ~ Speaker (-1)
    You leave Dakota be, reminded of why you two used to butt heads so much. What a pain...
    -> END

=== Dakota_Antonio ===

    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Do you know a man named Antonio Lenarduzzi?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Hmm... doesn't ring a bell. Is this guy a lead?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    A murder victim.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Ah, I see... that's unfortunate. 
    ~ Speaker (-1) 
    You two always had a habit of talking about serious matters such as this a bit too casually. You can't bring yourself to count it as evidence against Dakota.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    You know the how and where yet?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Mhmm. Lots of stab wounds to the heart. Happened in the aquarium.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Aaah, that would explain why it's locked down. Shame, they owe me a lot of money. Guess I won't be getting it anytime soon.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    An aquarium owing you money? How in the world did you wrap them around your finger?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Nothing underhanded, don't give me that look. I own the building, became a landlord since I left the force. They're just overdue on rent.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    A landlord... ew.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    It pays well. And I'd like to believe I treat people kindly enough. Don't look so glum about it.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    You used to be way cooler, man.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    And you used to not reek of cigarette smoke.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    Fuck you.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Fuck you.
    ~ Speaker (-1)
    You storm off, reminded of why you two broke u-
    ahem,
    stopped working together.
        -> END
    
    //QUEST - if player has investigated === fence_locked === prior to ever interacting with Dakota, automatically trigger this dialogue upon ending conversation with him. 
    // If player has not interacted with === fence_locked === prior, this dialogue does not trigger automatically, but rather is the dialogue that will play upon interacting with Dakota a second time after interacting with === fence_locked ===
    === Dakota_Key ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_LANDLORD, SPEAKER_SECONDARY)
    ~ Speaker (-1)
    Working yourself up over your conversation with Dakota, you realise there's a favour you need to ask him and begrudgingly turn to face him.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    GHH...
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    Dakota...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    What.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    Do you have the key for the fence in the alleyway? The one blocking off the breaker room.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Yeah. 
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    ...
    If you let me borrow it and don't end up being a murderer I'll treat you to lunch at the old diner by the station. For old times sake or whatever...
    ~ Speaker (-2)
    He looks away from you, and ponders to himself for a moment.
    He tosses you the keys without a word.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    ...
    
    -> END
    
    //BAKERY ------------------------------------------------------------------------------------------------------------------------
=== Briar_Meet ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_BAKER, SPEAKER_SECONDARY)

~ Character(CHAR_BAKER, 1)
~ Speaker(1)
~ Alias(1, "Baker")
Hello dear! Welcome to Baked Hearts Patisserie! How can I help you?

* [I'll get the daily special.] 
    -> Briar_Special
* [What would you recommend.] 
    -> Briar_Recommendations
* [I was actually hoping to ask you some questions.] 
    -> Briar_Questions

=== Briar_Special ===
    ~ Speaker(-2)
    You notice a board by the counter advertising today's special: A toasted croissant with hazelnut filling.

    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    I'll just get the daily special. Oh, and a black coffee, take away if it's possible.

    ~ Speaker(-2)
    You intentionally gaze towards the taped off door as you speak.

    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    A lovely choice!
    Oh- ahaha, don't worry yourself too much about that dear, nothing to be scared of. We've been given the clear to stay open for business, I heard the crime- poor thing- happened on the opposite side of the building.
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Ah, I see. Seems to have affected business though, this street is usually far more lively, isn't it?
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    Siiigh, definitely is... we tend to rely on the aquarium for most of our customers here,
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    but it's okay! I'm sure it'll be up and running again in no time. People cant resist a sweet treat for too long!
    ~ Speaker (-2)
    You smile and nod reassuringly at her chipper attitude, perhaps she'd be willing to keep answering questions while she's in a good mood.
    
       -> Briar_Questions

=== Briar_Recommendations ===
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Hmm, what would you recommend? I think I can trust your judgement, you seem like you have good taste.
    You shoot her a grin as you speak. Seriously not the time for flirting but okay.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_FLUSTER)
    Oh! Ehe, well I guess I know my way around here.
    I know it sounds like I'm taking the easy way out, but I'm serious when I say today's daily special is great.
    ~ Speaker(-2)
    She leans in over the counter and whispers closer to you.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    It's because I choose all the daily specials and pick my favourites you see, teehee.
    I make the hazelnut filling myself actually, it's my specialty!

    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Sounds lovely, consider me sold. I'll grab a black coffee with that as well if I can, takeaway if that's alright.

    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Of course dear! Just one moment.
    ~ Speaker (-1)
    You wait for the baker to prepare your order, awkardly looking around the establishment to avoid pressuring her. She eventually brings it to the counter and you thank her with a nod.
    
    -> Briar_Questions

    
=== Briar_Questions ===

    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Actually, I was wondering if I could ask you something?
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    Oh? Me? Well, I'll answer as best as I can.
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Are you feeling okay working so close to a crime scene? I'd be a little on edge if it were me.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Aw, I appreciate your concern, dear.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    I guess business has to run as usual... my boss was very determined to stay open once permission was given, but you're right it's a bit of an... odd feeling. None of my coworkers wanted to step in, and I'd hate to upset the regulars, so it's just me here today actually. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    But I'll be just fine!
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Of course. I think you're brave for stepping up to the task, if it's any consolation.
    ~ Speaker(-2)
    She giggles and waves her hand dismisivelly.
    Almost a bit too dismissively...
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    OH! I still need to pay, my bad. 
    
    ~ Speaker (-2)
    You start to rummage through your pockets to pay with your 
    * [Credit Card] 
    -> Briar_Card
    * [cash] 
    -> Briar_Cash
    * [Shit ton of loose coins] 
    -> Briar_Coins
    
    
    === Briar_Card ===
    ~ Speaker (-2)
    You fish out your credit card and swipe it across the pinpad. You select the option to tip as well when you do because you appreciate hospitality workers, and also this game takes place in a vaguely American setting so it's expected of you. 
    -> Briar_Name
    
    
    === Briar_Cash ===
    ~ Speaker (-2)
    You find your tattered leather wallet and pull out some notes. The baker takes them with a smile and begins to calculate your change.
    
    ~ Speaker (0) 
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Don't worry about it, keep the change. 
    
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Aww, are you sure? Thank you!
    
    ~ Speaker (0) 
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    No worries. 
    -> Briar_Name
    
    === Briar_Coins ===
    ~ Speaker (-2)
    You awkwardly keep rummaging for any notes in your pockets. There are none. You start to feebly pull out handfuls of loose change.
    So much loose change... an egregious amount of it.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    ..................
    ~ Speaker (-2)
    You slowly push the pile of coins towards the baker. Why would you do this to a minimum wage worker.
    
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    I'm so sorry...
    
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    That's alright, dear.
    -> Briar_Name
    
    
    

=== Briar_Name ===

    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Thank you. Oh by the way, a bit of an odd question, final one I swear, but could I get your name? I just wanted to put in a good review since you've been so nice to me.

    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Aww you're too kind, you don't need to do all that. Still, the name's Briar.
    ~ Alias(1, "")
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    And in exchange, could I get yours?

    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    You can call me Juno.

    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)

    Well I hope you'll come visit again, Juno. I'd love to chat some more but you'd be surprised how much work needs to be done behind the scenes even on quiet days like this.

~ Speaker(-1)
She waves you goodbye with a bright smile before leaving out the back of the store.

//QUEST - fade out to same scene without Briar in it.
        -> END



/* QUEST - The following prerequisites should be completed prior to Briar's sprite appearing in front of the cafe for the date segment.
- Completed prior interaction with Briar
- interacted with Dakota and obtained key
- unlocked breaker area
- activated lights in the aquarium*/

=== Briar_Date ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_BAKER, SPEAKER_SECONDARY)
    ~ Speaker (-1)
    Briar seems to be searching for something on the ground outside the cafe as you approach her.
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Fancy seeing you again so soon.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    Ah! Juno! Hello again dear.
    ~ Speaker (-2)
    She doesn't seem particularly ecstatic that you've interrupted her search.
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    You looking for something? I can help out if you'd like.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    That's sweet of you. It's just a little hairclip I'm fond of, a pink heart with angel wings.
    ~ Speaker (-2)
    The two of you rummage around the area for a bit looking for the hairclip. At some point you both reach for the same scrap of pink paper on the ground and your hands briefly touch. Kya!
    But there's no luck. 
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    Well, that's a shame.
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    At the very least, it's made me appreciate the area a little more. This really is a cute cafe.
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    I wouldn't mind going for a bite there with someone if you catch my drift.
    ~ Speaker(1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_FLUSTER)
    ..!
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Ehehe, well with someone as lovely as you I wouldn't mind a quick bite.
    ~ Speaker (-2)
    The two of you enter the cafe together, like a normal pair of youths on a first date rather than a detective and a suspect.
    //QUEST - fade black, fade in cafe 'smooth' background
    You take a seat and order a drink and a slice of cake. You sit across from Briar and make awkward small talk until your food arrives.
    Now, the time has come... You've successfully lured the suspect into a calm environment, it's time to get some answers out of her. Just remember to be subtle, this is just a date to her, after all.
    ~ Speaker(0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Sooo....
    
    * [Lovely weather we're having?] 
    -> Date_Weather
    * [How do you like working in this area?] 
    -> Date_Area
    * [What got you into baking?] 
    -> Date_Baking
    
    
=== Date_Weather ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    Lovely weather we're having?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Ehehe~
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Wha= What's so funny?
     ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    It's just a bit of a bland conversation starter, you were speaking a lot more confidently before.
    Are you nervous now?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_FLUSTER)
    N-No! Not at all!
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    It really is quite nice in this area. I haven't been to a seaside cafe before, the sea breeze coming in it soothing. 
    Is it a similar vibe at your bakery? y'know, when there isn't a crime scene right next door...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Oh yes, I love the atmosphere at Baked Hearts! I don't get the smell of the ocean as much though, it's overpowered by the aroma of freshly baked goods.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    It really is a dream job for me, I've been into baking ever since I was a little girl. I just love making things for others, the smiles on their faces when they take a bite makes my heart grow ten sizes!
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Like for friends and family? Or customers in general?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    I'd say both, but people you know closely are more rewarding to give gifts too. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    I used to bring a bunch to my old boyfriend... but things didn't work out sadly.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Oh! I'm sorry to hear that.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    It's nothing for you to apologise for dear.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Besides he... wasn't the most loyal of people. My efforts are better spent elsewhere.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Like on you, maybe! Heheh.
    -> Date_Progress
    
    
=== Date_Area ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    How do you like working in this area?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Oh it's lovely! Working by the ocean is always nice. It's not as strong at Baked Hearts as it is here, but the faint waft of the seas breeze is so refreshing.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Location aside, it really is a dream job for me! I've been into baking ever since I was a little girl. I just love making things for others, the smiles on their faces when they take a bite makes my heart grow ten sizes!
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Like for friends and family? Or customers in general?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    I'd say both, but people you know closely are more rewarding to give gifts too. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    I used to bring a bunch to my old boyfriend... but things didn't work out sadly.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Oh! I'm sorry to hear that.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    It's nothing for you to apologise for dear.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Besides he... wasn't the most loyal of people. My efforts are better spent elsewhere.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Like on you, maybe! Heheh.
    -> Date_Progress
    
=== Date_Baking ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    What got you into baking? 
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    I've been into baking ever since I was a little girl. I just love making things for others, the smiles on their faces when they take a bite makes my heart grow ten sizes!
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Like for friends and family? Or customers in general?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    I'd say both, but people you know closely are more rewarding to give gifts too. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    I used to bring a bunch to my old boyfriend... but things didn't work out sadly.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Oh! I'm sorry to hear that.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    It's nothing for you to apologise for dear.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    Besides he... wasn't the most loyal of people. My efforts are better spent elsewhere.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Like on you, maybe! Heheh.
    -> Date_Progress
    
    

==== Date_Progress ====
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    (I need to figure out where she was on the night of the murder...)

    * [What times do you usually work?] 
    -> Date_Work
    * [How does the collaboration with the aquarium work?] 
    -> Date_Aquarium
    * [You look sad, do you wanna talk about your ex?] 
    -> Date_Antonio
    
    
=== Date_Work ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    What times do you usually work? I noticed on the sign outside that the bakery's open pretty late.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Usually midday to close. I prefer closing shifts though, they're quieter. And then I can quickly run deliveries if needed. Like to this cafe!
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    I'll let you in on a little secret. We stock some of the pastry bases here. And occassionally cupcakes that we discontinued at Baked Hearts because they were harder to store alongside the pastries we keep warm all day in the cabinet.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    Oh really? That's cool! Is the stuff we're making right now part of one of your deliveries?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Hmmm... not these ones, no. I was meant to deliver some things recently but got... held up. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Oh I guess I can tell you. I'm just such a clutz! I tripped on my way here and damaged the baked goods, ehe.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    The owner was very kind and understanding about it, said it's fine and they weren't surprised I could trip when walking alone in the dark. So much kinder about little blunders like that than... other people that I know.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    (She's probably referring to that ex again...)

    -> Date_Antonio
    
    
=== Date_Aquarium ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    How's the collaboration with the aquarium next door to you work? It's a bit of an odd combo.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    Tell me about it! I heard it's because the landlord for the bakery is the same one as the aquarium and that they didn't want to demolish the pathway betwee the two buildings. You'd think they'd try to sell it as one complex though.
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    I can't deny that it's good for business though. We get many more kids dragging their parents in for a sweet treat than we would otherwise.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_ANGRY)
    Yeah I know the guy who runs this area, I'm not surprised he'd try to double his profits leasing them seperately. 
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    Area? It's not just those two buildings?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Mhmm, he owns the buildings on this street too. This cafe included.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    Ooooh! That would explain why the keys are similar! 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_NEUTRAL)
    See, I was running a delivery last night- we stock some of the pastry bases here, and I mixed up the keys and tried unlocking the back door with the bakery key. For a moment I thought it was the right one since it went most of the way in, but it wouldn't turn. But I guess that makes sense if that friend of yours uses matching locks for all the buildings.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    I wouldn't exactly call him a friend...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    An ex?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    ...
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Oh sweetie I can see it written all over your face. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    I get it... I've been there with the bitter feelings of it all. The less than savoury thoughts...
    -> Date_Antonio

=== Date_Antonio ===
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    Could I... ask about that ex of yours?
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    ...
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    Goodness, it really isn't the most appealing of topics for a first date, don't you think?
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_NEUTRAL)
    It seems to be weighing on your mind though.
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    I'd be happy to lend an ear.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    ...
    It's just... I feel like I did everything for him... I brought him treats to work everyday, I planned fun dates, I made sure to balance my work life around HIS schedule. 
    ~ Portrait (SPEAKER_SECONDARY, MOOD_ANGRY)
    And then the bastard went and cheated on me!
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SAD)
    Oh I should've listened to the others... my parents, my coworkers, even that clown who works in the aquarium said he was no good, and that was my only conversation with the guy! 
    I gave him all my love and that's how he repays me... good riddance.
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SHOCK)
    ..!
    (She could just be referring to breaking up with him but...)
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_SHOCK)
    Juno? You look like you've seen a ghost...
    ~ Speaker (0)
    ~ Portrait (SPEAKER_PRIMARY, MOOD_SAD)
    Ah, it's just I'm shocked someone could be so cruel...
    ~ Portrait (SPEAKER_PRIMARY, MOOD_HAPPY)
    But I'm glad you realised your own worth and left him once you found out.
    ~ Speaker (1)
    ~ Portrait (SPEAKER_SECONDARY, MOOD_HAPPY)
    Ehehe, yeah. That's what I did..!
    -> Date_Conclude
    


=== Date_Conclude ===
    ~ Speaker (-2)
    The two of you continue to converse as time escapes from you. You learn more about Briar's passion and how she turned it into a career, some anecdotes from her childhood, gossip she's gathered from her coworkers, all mundanely delightful things if you ignore your roles in this case.
    Eventually you part ways, as courtesy you pay the bill, the least you could do before you possibly put her behind bars...
    You should make your way back to your office and go over all the evidence you have gathered.
    -> END

































        -> END

