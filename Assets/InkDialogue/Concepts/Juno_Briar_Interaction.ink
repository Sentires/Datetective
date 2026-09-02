/*(Im sorry this isnt coded properly, ive got no clue how you wanted done but i hope it helps with getting started)*/

// I added this, it didn't have a knot before - Jack
=== interaction_enter ===
~ Character(CHAR_DETECTIVE, 0)
~ Speaker(-1)
You enter the bakery next door to the crimescene. Despite the horrific act that has taken place so near by, it's still open for business, though notably the entrance connecting the store directly to the aquarium has been barred off with police tape.
Unsurprisingly, there are few customers in sight at a time like this.
Behind the counter stands a young lady, one who's face aligns with one of the three suspects you had been given information on. 

~ Character(CHAR_BAKER, 1)
~ Speaker(1)
Cafe Worker: Hello dear! Welcome to Baked Hearts Patisserie & Cafe. How can I help you?

* [I'll get the daily special.] 
    -> special
* [What would you recommend.] 
    -> recommendations
* [I was actually hoping to ask you some questions.] 
    -> questions

=== special ===
~ Speaker(-1)
You notice a board by the counter advertising today's special: A toasted croissant with hazelnut filling.

~ Speaker(0)
Juno: I'll just get the daily special. Oh, and a black coffee, dining in if it's possible.

~ Speaker(-1)
You intentionally gaze towards the taped off door as you speak.

~ Speaker(1)
Cafe Worker: A lovely choice! 

~ Speaker(-1)
She catches your eye after your gaze returns from the door.

~ Speaker(1)
Cafe Worker: Oh- ahaha, don't worry yourself too much about that dear... we've been given the clear to stay open for business, I heard the crime- poor thing- happened on the opposite side of the building. Make yourself comfortable wherever you like, I'll bring it out to you soon enough!

~ Speaker(-1)
You pay for you your meal and find a place to sit.
* [Take a seat]
    -> Table

=== recommendations ===
~ Speaker(0)
Juno: Hmm, what would you recommend? I think I can trust your judgement, you seem like you have good taste.
You shoot her a grin as you speak. Seriously not the time for flirting but okay.

~ Speaker(1)
Cafe Worker: Oh! Ehe, well I guess I know my way around here.
Cafe Worker: I know it sounds like I'm taking the easy way out, but I'm serious when I say today's daily special is great.

~ Speaker(-1)
She leans in over the counter and whispers closer to you.

~ Speaker(1)
Cafe Worker: It's because I choose all the daily specials and pick my favourites you see, teehee.
Cafe Worker: I make the hazelnut filling myself actually, it's my specialty.

~ Speaker(-1)
There's a glimmer of pride in her eyes as she tells you her little secret.

~ Speaker(0)
Juno: Sounds lovely, consider me sold. I'll grab a black coffee with that as well if I can.

~ Speaker(1)
Cafe Worker: Of course dear! Make yourself comfortable and I'll bring it right over.

~ Speaker(-1)
You pay for you your meal and find a place to sit.
* [Take a seat]
    -> Table

=== questions ===
~ Speaker(1)
Cafe Worker: Oh? Me? Well, I can answer as best as I can I guess, so ask away.
There was a flicker of surprise in her eyes, but she quickly regained her composure.

~ Speaker(0)
Juno: I'm surprised this place is open is all. Are you feeling okay working so close to a crime scene? I'd be a little on edge if it were me.

~ Speaker(1)
Cafe Worker: Oh that... I guess business has to run as usual ahaha... my boss was very determined to stay open once permission was given, but you're right it's a bit of an... odd feeling. None of my coworkers wanted to step in, and I'd hate to upset the regulars, so it's just me here today actually. 
Cafe Worker: Thank you for your concern though, that's sweet of you.

~ Speaker(0)
Juno: Of course. I think you're brave for stepping up to the task, if it's any consolation.

~ Speaker(-1)
She giggles and waves her hand dismisivelly.

~ Speaker(1)
Cafe Worker: Ah, it could be worse I guess... anyway! Could I get you anything today, dear?

~ Speaker(0)
Juno: Oh yes- my bad, I'll just get the...

~ Speaker(-1)
You frantically look around for somethin to order, and notice the daily special, a toasted croissant with hazelnut filling.

~ Speaker(0)
Juno: Just today's special and a black coffee, please.

~ Speaker(1)
Cafe Worker: Perfect! I'll get that ready right away for you, dear.

~ Speaker(-1)
You pay for you your meal and find a place to sit.
* [Take a seat]
    -> Table
=== Table ===
~ Speaker(-1)
Eventually, the lady who had taken your order brings it to the table. She smiles warmly at you as she places it down with steady hands, not a stray crumb on the plate or any ripples on the coffee's surface.
Despite knowing her name from the file you had received on her, it would still be polite (and far less suspicious) to ask.

~ Speaker(0)
Juno: Thank you. Oh by the way, a bit of an odd question but could I get your name? just wanted to put in a good review since you've been so nice to me.

~ Speaker(1)
Cafe Worker: Aww you're too kind, you don't need to do all that. Still, the name's Briar.
Briar: And in exchange, could I get yours?

~ Speaker(0)
Juno: You can call me Juno.

~ Speaker(1)
Briar: Well I hope you'll come visit again, Juno. I'd love to chat some more but you'd be surprised how much work needs to be done behind the scenes even on quiet days like this.

~ Speaker(-1)
She waves you goodbye with a bright smile, leaving you to your meal.


-> END
