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
~ Character(Char_Detective, SpeakerIndex_Primary)
~ Character(Char_Clown, SpeakerIndex_Secondary)
~ Portrait(SpeakerIndex_Primary, Mood_Neutral)
~ Portrait(SpeakerIndex_Secondary, Mood_Neutral)

~ Speaker(SpeakerIndex_Secondary)

(Say something to Juno)
+[Introduction] -> Juno_Introduction
+['What are your likes and dislikes?'] -> Juno_Likes_Dislikes
+['You're cute'] -> Juno_Flirt
+['I don't think you're telling me the whole truth...'] -> Juno_Confrontation

=== Juno_Introduction ===
~ Speaker(SpeakerIndex_Primary)
~ Portrait(SpeakerIndex_Primary, Mood_Happy)
~ Portrait(SpeakerIndex_Secondary, Mood_Happy)
Name's Juno. Pleasure's all mine.

I just happened to be in the area, couldn't help but be a little curious with what happened here. What about you?
-> Juno

=== Juno_Likes_Dislikes ===
~ Speaker(SpeakerIndex_Primary)

~ Portrait(SpeakerIndex_Secondary, Mood_Happy)
Juno: A rather straightforward way to get to know me, huh?

~ Portrait(SpeakerIndex_Primary, Mood_Happy)
Hmm... I guess you could say I'm into older media and hardware. I like the practicality of dial up phones and taking notes by hand. There's just something about the feeling of it that modern technology doesn't have... also touchscreens suck when you like to wear gloves, haha...

~ Portrait(SpeakerIndex_Primary, Mood_Sad)
~ Portrait(SpeakerIndex_Secondary, Mood_Angry)
They adjust their aforementioned gloves while they speak, seemingly looking a little bitter about the ordeal they choose to put themselves through.

~ Portrait(SpeakerIndex_Primary, Mood_Happy)
~ Portrait(SpeakerIndex_Secondary, Mood_Neutral)
Juno: 'Guess that answers what I'm not fond of too. It's not just about the tech, but also the human connection you can lose 'cos of it. I'd much rather speak to someone face to face over a coffee than an online meeting. You get me?'
-> Juno

=== Juno_Flirt ===
~ Speaker(SpeakerIndex_Primary)
~ Portrait(SpeakerIndex_Primary, Mood_Shock)
~ Portrait(SpeakerIndex_Secondary, Mood_Fluster)
Juno: '..! Heh, you really get to the point, don't you. Th-thanks... you're not winning me over that easily though.'

~ Portrait(SpeakerIndex_Primary, Mood_Fluster)
Despite trying to play it off, Juno's cheeks are visibly redder than before.
-> Juno

=== Juno_Confrontation ===
~ Speaker(SpeakerIndex_Primary)

~ Portrait(SpeakerIndex_Primary, Mood_Angry)
~ Portrait(SpeakerIndex_Secondary, Mood_Shock)
Juno: '...

~ Portrait(SpeakerIndex_Primary, Mood_Sad)
I will when the time is right. You trust me, don't you?'

~ Portrait(SpeakerIndex_Secondary, Mood_Sad)
Their tone is reassuring, but you've never seen their face so serious before.

-> END //Ending instead of looping?

    


