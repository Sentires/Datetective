/*CHARACTERISATION BREAKDOWN - Juno
(Note: Juno avoids mentioning their role as a detective within the narrative, hence why that information is obscured here as well.)

Basic info I like to write out to ground the character

Name: Juno Coman
Age: 27
Ethnicity: Romani
Height: 5'7" / 171cm
Pronouns: they/he/she
Occupation: Private investigator, former police officer*/
//-> Juno

// Entrance Knot
=== Juno ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Character(CHAR_CLOWN, SPEAKER_SECONDARY)
~ Alias(1, "Ceasar")
~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)

~ Speaker(SPEAKER_SECONDARY)

(Say something to Juno)
+[Introduction] -> Juno_Introduction
+['What are your likes and dislikes?'] -> Juno_Likes_Dislikes
+['You're cute'] -> Juno_Flirt
+['I don't think you're telling me the whole truth...'] -> Juno_Confrontation

=== Juno_Introduction ===
~ Speaker(SPEAKER_PRIMARY)
~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
~ Portrait(SPEAKER_SECONDARY, MOOD_HAPPY)
Name's Juno. Pleasure's all mine.

I just happened to be in the area, couldn't help but be a little curious with what happened here. What about you?
-> Juno

=== Juno_Likes_Dislikes ===
~ Speaker(SPEAKER_PRIMARY)

~ Portrait(SPEAKER_SECONDARY, MOOD_HAPPY)
Juno: A rather straightforward way to get to know me, huh?

~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
Hmm... I guess you could say I'm into older media and hardware. I like the practicality of dial up phones and taking notes by hand. There's just something about the feeling of it that modern technology doesn't have... also touchscreens suck when you like to wear gloves, haha...

~ Portrait(SPEAKER_PRIMARY, MOOD_SAD)
~ Portrait(SPEAKER_SECONDARY, MOOD_ANGRY)
They adjust their aforementioned gloves while they speak, seemingly looking a little bitter about the ordeal they choose to put themselves through.

~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)
Juno: 'Guess that answers what I'm not fond of too. It's not just about the tech, but also the human connection you can lose 'cos of it. I'd much rather speak to someone face to face over a coffee than an online meeting. You get me?'
-> Juno

=== Juno_Flirt ===
~ Speaker(SPEAKER_PRIMARY)
~ Portrait(SPEAKER_PRIMARY, MOOD_SHOCK)
~ Portrait(SPEAKER_SECONDARY, MOOD_FLUSTER)
Juno: '..! Heh, you really get to the point, don't you. Th-thanks... you're not winning me over that easily though.'

~ Portrait(SPEAKER_PRIMARY, MOOD_FLUSTER)
Despite trying to play it off, Juno's cheeks are visibly redder than before.
-> Juno

=== Juno_Confrontation ===
~ Speaker(SPEAKER_PRIMARY)

~ Portrait(SPEAKER_PRIMARY, MOOD_ANGRY)
~ Portrait(SPEAKER_SECONDARY, MOOD_SHOCK)
Juno: '...

~ Portrait(SPEAKER_PRIMARY, MOOD_SAD)
I will when the time is right. You trust me, don't you?'

~ Portrait(SPEAKER_SECONDARY, MOOD_SAD)
Their tone is reassuring, but you've never seen their face so serious before.

-> END //Ending instead of looping?

    


