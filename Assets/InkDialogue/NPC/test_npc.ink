// VARS //
// questID + "Id" for naming
VAR TestingQuestId = "TestingQuest"
// questID + "State" for naming
VAR TestingQuestState = QuestState_Unavailable


=== Juno_Test_Talksprites ===
~ Character(CHAR_DETECTIVE, SPEAKER_PRIMARY)
~ Speaker(SPEAKER_PRIMARY)

~ Portrait(SPEAKER_PRIMARY, MOOD_NEUTRAL)
Mood = Neutral/{MOOD_NEUTRAL}

~ Portrait(SPEAKER_PRIMARY, MOOD_HAPPY)
Mood = Happy/{MOOD_HAPPY}

~ Portrait(SPEAKER_PRIMARY, MOOD_SHOCK)
Mood = Shock/{MOOD_SHOCK}

~ Portrait(SPEAKER_PRIMARY, MOOD_SAD)
Mood = Sad/{MOOD_SAD}

~ Portrait(SPEAKER_PRIMARY, MOOD_ANGRY)
Mood = Angry/{MOOD_ANGRY}

~ Portrait(SPEAKER_PRIMARY, MOOD_FLUSTER)
Mood = Fluster/{MOOD_FLUSTER}
 
-> END

=== Briar_Test_Talksprites ===
~ Character(CHAR_BAKER, SPEAKER_SECONDARY)
~ Speaker(SPEAKER_SECONDARY)

~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)
Mood = Neutral/{MOOD_NEUTRAL}

~ Portrait(SPEAKER_SECONDARY, MOOD_HAPPY)
Mood = Happy/{MOOD_HAPPY}

~ Portrait(SPEAKER_SECONDARY, MOOD_SHOCK)
Mood = Shock/{MOOD_SHOCK}

~ Portrait(SPEAKER_SECONDARY, MOOD_SAD)
Mood = Sad/{MOOD_SAD}

~ Portrait(SPEAKER_SECONDARY, MOOD_ANGRY)
Mood = Angry/{MOOD_ANGRY}

~ Portrait(SPEAKER_SECONDARY, MOOD_FLUSTER)
Mood = Fluster/{MOOD_FLUSTER}
 
-> END

=== Ceasar_Test_Talksprites ===
~ Character(CHAR_CLOWN, SPEAKER_SECONDARY)
~ Speaker(SPEAKER_SECONDARY)

~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)
Mood = Neutral/{MOOD_NEUTRAL}

~ Portrait(SPEAKER_SECONDARY, MOOD_HAPPY)
Mood = Happy/{MOOD_HAPPY}

~ Portrait(SPEAKER_SECONDARY, MOOD_SHOCK)
Mood = Shock/{MOOD_SHOCK}

~ Portrait(SPEAKER_SECONDARY, MOOD_SAD)
Mood = Sad/{MOOD_SAD}

~ Portrait(SPEAKER_SECONDARY, MOOD_ANGRY)
Mood = Angry/{MOOD_ANGRY}

~ Portrait(SPEAKER_SECONDARY, MOOD_FLUSTER)
Mood = Fluster/{MOOD_FLUSTER}

// Custom talksprites indices below
~ Portrait(SPEAKER_SECONDARY, MOOD_SPECIAL_NEUTRAL)
Mood = Custom Neutral/{MOOD_SPECIAL_NEUTRAL}

~ Portrait(SPEAKER_SECONDARY, MOOD_SPECIAL_SHOCK)
Mood = Custom Shock/{MOOD_SPECIAL_SHOCK}
 
-> END

=== Dakota_Test_Talksprites ===
~ Character(CHAR_LANDLORD, SPEAKER_SECONDARY)
~ Speaker(SPEAKER_SECONDARY)

~ Portrait(SPEAKER_SECONDARY, MOOD_NEUTRAL)
Mood = Neutral/{MOOD_NEUTRAL}

~ Portrait(SPEAKER_SECONDARY, MOOD_HAPPY)
Mood = Happy/{MOOD_HAPPY}

~ Portrait(SPEAKER_SECONDARY, MOOD_SHOCK)
Mood = Shock/{MOOD_SHOCK}

~ Portrait(SPEAKER_SECONDARY, MOOD_SAD)
Mood = Sad/{MOOD_SAD}

~ Portrait(SPEAKER_SECONDARY, MOOD_ANGRY)
Mood = Angry/{MOOD_ANGRY}

~ Portrait(SPEAKER_SECONDARY, MOOD_FLUSTER)
Mood = Fluster/{MOOD_FLUSTER}

 
-> END

// DIALOGUE //
=== TestingQuest ===
{ TestingQuestState :
    - QuestState_Unavailable: -> Unavailable
    - QuestState_Available: -> Available
    - QuestState_Active: -> Active
    - QuestState_Achieved: -> Achieved
    - QuestState_Concluded: -> Concluded
    - else: -> END
}


= Unavailable
{QuestState_Unavailable_Debug}
-> END
= Available
{QuestState_Available_Debug}
* [Accept the quest] You have started the quest!
~ StartQuest(TestingQuestId)
-> END
= Active
{QuestState_Active_Debug}
* [Work on the quest] You have worked on the quest!
~ AdvanceQuest(TestingQuestId)
-> END
= Achieved
{QuestState_Achieved_Debug}
* [Finish on the quest] You have completed on the quest!
~ FinishQuest(TestingQuestId)
-> END
= Concluded
{QuestState_Concluded_Debug}
-> END

/*
~ temp targetQuest = "Test 1"
~ StartQuest(targetQuest)
How do you like to spend your free time, dear?
* [Case related & obvious answer]
    \+1 Evidence, +1 Suspicion and -1 Affection
* [Neutral answer]
    \+1 or -1 Affection
* [Case related & non-suspicious answer]
    \+1 Evidence
* [Suspect pandering]
    \+2 Affection
- // Gather here
    
~ AdvanceQuest(targetQuest)
* [Wowee neato!!!!] 
    I'm so cool
- // Gather here
    
Ok now go away, vermin!
~ FinishQuest(targetQuest)
-> END
*/