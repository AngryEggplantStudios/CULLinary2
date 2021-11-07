using UnityEngine;

// A store of all dialogue strings and their random weights.
// 
// Dialogue Format:
// text        - any string of characters (avoid using: {}<>[])
// number      - any string of digits
//
// sprite_id   - number
// num_choices - number
//
// dialogue    - (plain|choice)*
// plain       - { + ([L]|[R]) + sprite_id + } + text
// choice      - { + num_choices + } + ([C + , + text + ] + < + dialogue + >)*
//
// L for head sprite to be placed on the left
// R for head sprite to be placed on the right
// 
// Example:
// {[R]1}How are you today?{2}[C,Great (Happy)]<{[L]0}I'm great, thank you!>[C,Keep quiet (Rude)]<{[L]0}Keep quiet! Silence in the restaurant please.>
public static class DialogueDatabase
{
    // Add dialogues to this array with weights for random chance
    private static (string, int)[] rawDialoguesWithWeights = {
        /*    0 */ ("{[R]3}Ho ho ho, smells delicious!{[R]3}I swear, this is the only decent delivery service in town...", 10),
        /*    1 */ ("{[R]4}How are you today?{2}[C,Great]<{[L]0}I'm great, thank you!{[R]4}That's great!>[C,Keep quiet]<{[L]1}Keep quiet! Silence please, I'm very busy.{[R]4}Are you kidding me? I'm never ordering again.>", 10),
        /*    2 */ ("{[R]5}Can I see the manager?{3}[C,No]<{[L]1}No, you can't.{[R]5}What a horrible delivery service.>[C,I am the manager.]<{[L]2}You are speaking to the manager.{[R]5}Could I get a discount? It's my birthday...{2}[C,Yes]<{[R]5}Hooray!>[C,No]<{[R]5}Aww... Worst delivery service ever.>>[C,You are the manager.]<{[L]2}You are the manager.{[R]5}Wow! That's great! I've never held a job before.{[L]1}No, that's not... Never mind.>", 10),
        /*    3 */ ("{[R]6}After the plants started attacking, all my friends have moved out...{[R]6}You're the only one left who I can talk to. *sob*", 10),
        /*    4 */ ("{[R]7}Have you ever been to the factory?{2}[C,Yes]<{[L]2}Yep, I have.{[R]7}Oh, then you must have seen the chemical spills...{[R]7}The poor animals and plants...>[C,No]<{[L]2}Nope, should I?{[R]7}Oh. Don't go there! The chemical spills smell horrible.>", 10),
        /*    5 */ ("{[L]2}How's the food?{[R]8}It's so good! Thanks for asking.", 5),
        /*    6 */ ("{[L]2}How's the food?{[R]9}Is that eggplant?{[R]9}Uuf, I think I'm going to be sick...", 5),
        /*    7 */ ("{[R]10}I once had a dream about crystals in a forest.{[R]10}I had to collect all of them while zombies were chasing me.", 1),
        /*    8 */ ("{[R]11}I once had a dream where I was fighting this knight guy.{[R]11}It was snowy and I had to stand in a fire to warm up!", 1),
        /*    9 */ ("{[R]12}Have you ever had a dream where you were stuck in a room?{[R]12}I did and 3 other guys were trying to kill me with lasers...", 1),
        /*   10 */ ("{[R]13}What other dishes can you cook?{7}[C,Eggplant Salad]<{[L]2}How about an Eggplant Salad?{[R]13}You have to be kidding me.{[R]13}No more eggplants, please!!>[C,Salmorejo]<{[L]2}We serve salmorejo and other Andalusian dishes.{[R]13}Wow! Where do you even get the ingredients for that?!>[C,Nasi Lemak]<{[L]2}Would you like some nasi lemak?{[R]13}That's my favourite dish!{2}[C,Mine too!]<{[R]13}I've got to try your nasi lemak someday!>[C,I lied, I can't cook that]<{[L]2}Sorry, I don't actually know how to cook nasi lemak...{[R]13}It's okay. You can learn before I order the next time!>>[C,Instant Ramen]<{[L]0}I can cook instant ramen...{[R]13}That's not impressive! I can do that too!!>[C,Anuflora]<{[L]2}I can cook \"Anuflora\"...{[R]13}Never heard of that, what's in it?{3}[C,Apples and Oranges]<{[R]13}Wait, it's a fruit salad?>[C,Fish and Tomatoes]<{[R]13}Sounds good.>[C,There's no such thing]<{[R]13}I knew it! You liar!>>[C,Crispy Bacon]<{[L]2}I can fry up some bacon if you want.{[R]13}Uhh... That's okay... I'm a vegan.>[C,Foie Gras]<{[L]0}Mademoiselle, we serve the best foie gras... Delicieuse!{[R]13}Uhh... I'm afraid I don't have the money for that...>", 1),
        /*   11 */ ("{[L]2}How are you doing?{[R]14}Not so good...{[R]14}I had a nightmare where I was stuck in space...{[R]14}I had to place guns to fend off hostile invaders!", 1),
        /*   12 */ ("{[L]2}How are you doing?{[R]3}Okay, I guess.{[R]3}Other than that dream I had where I was stuck in a maze.{2}[C,What happened?]<{[R]3}There were chemical spills...{[R]3}I was being chased by some horrifying ghost thing... *shudder*>[C,Sorry to hear that.]<{[L]2}I'm sorry to hear that.{[R]3}Yeah, I had to control my heartbeat and catch glowing orbs to get out...>", 1),
        /*   13 */ ("{[R]4}Hi! Random question, have you been stuck in a laboratory?{2}[C,Can't say I have.]<{[R]4}If you do, bringing a drone might be helpful!>[C,Yes]<{[R]4}If you get stuck again, you should look for some letters!{[R]4}The owner of the lab may have left his password lying around...>", 1),
        /*   14 */ ("{[R]5}I once had a dream where everything was white.{[R]5}I had to find colour pools to restore the colour with my paint gun!", 1),
        /*   15 */ ("{[R]6}Rumour has it that the one behind this chaos is a clown-{[L]1}AAHHH!!{[R]6}...{[R]6}Wait, what did I say? I don't know what got into me there...{[R]6}Also, why do you look so frightened?{[L]2}Uhh... Never mind.", 2),
        /*   16 */ ("{[R]7}You're not from around these parts, are you?{[R]7}Where are you from?{8}[C,Doesn't matter]<{[L]2}It doesn't matter where I'm from.{[L]0}What matters is I'm here now!{[R]7}That's a nice attitude to have!{[R]7}But I was just curious...>[C,Africa]<{[L]2}I'm from Africa.{[R]7}Wow! Is it really true that every minute, 60 seconds pass in Africa?{[L]1}...>[C,Antarctica]<{[L]2}I'm from Antartica.{[R]7}Really! You must see a lot of polar bears!{[L]1}What?! Polar bears are not found at the south pole!>[C,Asia]<{[L]2}I'm from Asia.{[R]7}Ah! You must be very good at math.{[L]1}Uh... What?>[C,Australia]<{[L]2}I'm from Australia.{[R]7}Ooh... G'day mate! How many kangaroos have you caught?{[L]1}...>[C,Europe]<{[L]2}I'm from Europe.{[R]7}Ah! Guten tag! Bonjour!{[L]1}There's more than two countries in Europe...>[C,North America]<{[L]2}I'm from North America.{[R]7}Oh... The land of the free!{[L]1}Hey... America is not just the United States, okay?>[C,South America]<{[L]2}I'm from South America.{[R]7}Oh, I can speak Brazilian! Bom dia!{[L]1}Who says I'm Brazilian? Also, the language is Portuguese!>", 2),
        /*   17 */ ("{[R]8}Have you heard about the ancient curse of Corona?{[R]8}It is said to affect those who eat bats!{[L]1}What rubbish. We don't serve any bats here.{[R]8}Why not? They are a good source of protein!{[L]1}But you just said... Never mind...", 2),
        /*   18 */ ("{[R]9}The prophecies tell of the chosen one...{[R]9}\"Thou shall not feareth the cloyne...\"{[R]9}\"The blight did casteth upon ye land. Yield to purifying light!\"{[R]9}Any idea what that means?{3}[C,Nope]<{[L]2}Uh, nope?{[R]9}Shame. You could have been the chosen one!>[C,Yes, of course]<{[L]0}Of course! It means the soil will become more fertile soon.{[R]9}Really? Well that must be good for the farmers!>[C,Nonsense]<{[L]1}What nonsense. I don't believe in such things.{[L]2}I only believe in science.{[R]9}Oh... Well... Can your science explain why it rains?{[R]9}Checkmate, loser!{[L]1}YES! Yes it can!>", 2),
        /*   19 */ ("{[R]10}The rare golden eggplant used to be the speciality of the town.{[R]10}Do you serve it?{[L]2}We do! But it's a seasonal dish.{[R]10}Ok, let me know when you do have it!", 3),
        /*   20 */ ("{[R]11}We're no strangers to love...{[R]11}You know the rules and so do I...{[L]2}I'll leave you to your singing...{[R]11}A full commitment's what I'm thinking of...{[R]11}You wouldn't get this from any other guy!{[L]0}Please, stop.{[R]11}Iiiii just wanna tell you how I'm feeling...{[R]11}Gotta make you... understand!{[R]11}NeVer goNna giVe yOuuUuuU uPPP!{[L]2}Oh god.{2}[C,Run away]<{[R]11}Never goonnna let youUuuUU dow- Wait, where did my audience go?>[C,Stay and listen]<{[L]1}No! Why would you pick that?!{[R]11}Never goonnna let youUuuUU downnnn...{[R]11}Never gonna ruuunn arooundDd and... deSeERRt yoOOu!{[R]11}Never gonnnaa maaake you cryyy...{[R]11}Never goonnna say gooodbye!{[R]11}Never gonna teeeelll a lieeee and huurt yooUuuuU!{[L]1}You are officially banned from my delivery service.>", 1),
        /*   21 */ ("{[R]12}I love eggplants!{[L]2}That's great! We only serve eggplants to you.{[R]12}This is the best delivery service!", 3),
        /*   22 */ ("{[R]13}Have you ever thought about the effects of your actions on the environment?{[R]13}You may be driving the local species to extinction!{[L]2}Not really...{[R]13}I've been seeing lesser cranes and cactuses in the area.{[R]13}They are endangered species, if you didn't know!{[L]0}Oh, that's not my fault! I only look for edible ingredients.{[R]13}Who says that cactuses and cranes aren't edible?{[L]1}What! I never serve them as food!{[R]13}Well... There's always a supply if the demand is high enough...{[L]1}I know who is actually responsible for the ruined ecosystem now!", 1),
        /*   23 */ ("{[R]3}I had a dream that I was trapped in a giant box!{[R]3}I was stuck in the desert with many enemies and had to deal with a shopkeeper that couldn't spell.{[L]2}Was there a cactus?{[R]3}How did you know?", 1),
        /*   24 */ ("{[R]4}Hey! Do you know 2 other people?{[L]2}Why?{[R]4}I invented a new game!{[R]4}You have to dodge spinning lasers that are controlled by others.{[R]4}Or, you could cross a gap where others are controlling the platforms!{[L]2}Sounds like a recipe for disaster to me!{[R]4}Ooh! Is that a dish I can order?{[L]1}No, it's a figure of speech!", 1),
        /*   25 */ ("{[L]2}Hey! I recognise you!{[L]0}Tew Tawrel, man! How's it going?{[R]5}What? What did you call me?{3}[C,Tutorial man]<{[L]2}Aren't you the tutorial man, man?{[R]5}Hmm... You must be talking about my twin brother!{[L]0}Oh, that explains it!>[C,I must be mistaken]<{[L]2}Oh sorry, I must have mistaken you for someone else.{[R]5}It's alright! It happens often.>[C,Do you have a twin]<{[L]2}Uh, do you have a twin brother?{[R]5}I do! My name is Edi. I work at The Aubergine Times!{[L]0}That explains it! Nice to meet you.{[R]5}To you as well!>", 6),
        /*   26 */ ("{[R]6}Why are you so late?{2}[C,Sorry]<{[L]2}Sorry, it's a jungle out there!{[R]6}It's okay. I'm just glad to get my food at all!{[L]0}Enjoy your food!{[R]6}I will, thanks!>[C,It's not my fault]<{[L]1}It's not my fault! My truck broke down!{[R]6}Next time, drive better!{[L]1}Why don't you buy your own food next time!>", 3),
        /*   27 */ ("{[R]7}Hey! I recognise you!{[R]7}You are the guy that keeps appearing on the news!{[L]2}I guess being the only chef on this island makes me a celebrity!", 5),
        /*   28 */ ("{[R]8}Finally! I've been waiting since forever!{[L]0}I don't think it's been that long!{[R]8}You're right, I just like making exaggerated statements to prove my point!{[L]2}I'm glad that you're honest about it, at least!", 3),
        /*   29 */ ("{[R]9}Watch out for the strange structures on the island!{[R]9}I have seen many wild vegetables gathered around them.{[R]9}I saw a chest as well, but it was too risky to check it out!{[L]2}Thanks for the advice!", 3),
        /*   30 */ ("{[R]10}Do you need any help with your delivery business?{[L]2}Thanks for asking, but I'm doing well at the moment!", 2),
        /*   31 */ ("{[R]11}Do you know why there are chests all over the island?{[L]0}I'm not sure!{[R]11}Rumour has it that the vegetables are starting their own capitalist system!{[R]11}They are amassing funds in the chests to overthrow the greedy human executives.{[L]2}That sounds ridiculous!{[R]11}I know!", 5),
        /*   32 */ ("{[R]12}Hi. Uhh, my food please?{[L]2}Where's the payment?{[R]12}Well, I could pay you in terms of exposure!{[L]1}No way! I'm taking this for myself.{[R]12}Wait, I'm sorry! I was kidding!{[L]1}Alright, no funny business this time!{[R]12}Actually, could I have a free drink to go along with my meal?{[R]12}Please, I will help you to advertise your business! I have 5 followers on Brinjagram and counting.{[L]1}...", 3),
        /*   33 */ ("{[R]13}Wait! I have something to say, don't go yet!{[R]13}Today's message is brought to you by our sponsor, CornVPN!{[R]13}Protect your privacy from prying eyes with military-grade encryption!{[L]1}I'm never delivering food to this house again.{[R]13}Remember to like, comment and subscribe, and don't forget to hit the notification bell!{[L]1}Good day to you! I do hope that the food displeases your olfactory system!", 3),
        /*   34 */ ("{[R]14}Hey! I just wanted to say that I really appreciate your delivery service!{[L]0}You are welcome!", 5),
        /*   35 */ ("{[R]14}Wait, I have a promotional code!{[L]0}Sorry... Our delivery business does not have those!{[R]14}Oh? But I got an email that said I won a free pizza!{[R]14}I just had to put in my credit card details to get it!{[L]1}What! That sounds like a scam!{[R]14}Oh? By the love of holy potatoes!{[L]2}You'd better call the bank!{[R]14}I will! I'm such an idiot. Thanks for the food!", 5),
    };

    private static string[] rawTutorialDialogues = {
        /* 0 */ "{[L]0}*Rrring rrring* Oh, looks like I already have an order coming in!",
        /* 1 */ "{[R]1}Hey Chef! Are you still open for business?"
                    + "{[L]0}Yes, I'm now open for delivery. What would you like to have, Tew Tawrel?"
                    + "{[R]1}That's great! I want some French fries, please. *Call ended*",
        /* 2 */ "{[L]0}I just got my first order! Let me check how many ingredients I need by pressing TAB to see my Orders and Recipes menus.",
        /* 3 */ "{[L]0}Okay, time to find me 3 potatoes by killing some potatoes. (Move around with WASD and attack with Left Mouse Button).",
        /* 4 */ "{[L]0}Now that I've collected 3 potatoes, I gotta find a campfire to cook the French fries. I can use the map to find a campfire.",
        /* 5 */ "{[L]0}Alright I got the French fries ready to go! Time to deliver it to my customer."
                    + "{[R]1}Hey Chef! You got my French fries yet?"
                    + "{[L]0}Yes! I was about to deliver it to you."
                    + "{[R]1}Great! Do you see the clock at the top? Just remember that you have to deliver your orders before the day ends at 23:59. *Call ended*",
        /* 6 */ "{[L]0}Okay, let's go deliver the French fries to Tew Tawrel! Gotta check the map to find out where his house is at.",
        /* 7 */ "{[R]1}Hey you're here!"
                    + "{[L]0}Yeap, I'm here to deliver your French fries."
                    + "{[R]1}Did you also know you could actually eat your own cooked food and get some special effects? How cool is that?!"
                    + "{[L]0}Oooh hmmmm... it sure looks tasty..."
                    + "{[R]1}Hey but those French fries are mine! If you eat my food, I'll get hangry >:("
                    + "{[R]1}Now take your money and gimme my French fries!"
                    + "{[L]0}Fine... here you go.",
        /* 8 */ "{[L]0}Looks like I can earn money from delivering orders and I can use the money to buy new weapons and other things to become stronger and complete more orders and earn EVEN MORE money...",
        /* 9 */ "{[L]0}I also need to keep an eye out for the newspaper, I've heard that more and more strange foods have been appearing lately... I wonder what's causing it...",
        /* 10 */ "{[L]0}Well, now I'm ready. Let's start CULLing!",
    };

    private static (Dialogue, double)[] dialoguesWithCumulativeChance = null;
    private static Dialogue[] generatedTutorialDialogue = null;
    private static bool hasGenerated = false;

    public static void GenerateDialogues()
    {
        int numberOfDialogues = rawDialoguesWithWeights.Length;
        dialoguesWithCumulativeChance = new (Dialogue, double)[numberOfDialogues];

        int totalWeight = 0;
        for (int i = 0; i < numberOfDialogues; i++)
        {
            int weight = rawDialoguesWithWeights[i].Item2;
            totalWeight = totalWeight + weight;
        }

        double currentWeight = 0.0;
        for (int i = 0; i < numberOfDialogues; i++)
        {
            (string rawDialogue, int weight) = rawDialoguesWithWeights[i];
            Dialogue parsedDialogue = DialogueParser.Parse(rawDialogue);
            if (parsedDialogue == null)
            {
                Debug.Log("Oops, dialogue #" + i + " is malformed!");
                break;
            }

            currentWeight = currentWeight + (double)weight / (double)totalWeight;
            double cumulWeightForI = currentWeight;
            dialoguesWithCumulativeChance[i] = (parsedDialogue, cumulWeightForI);
        }
        hasGenerated = true;
    }

    public static void GenerateTutorialDialogue()
    {
        int numDialogue = rawTutorialDialogues.Length;
        generatedTutorialDialogue = new Dialogue[numDialogue];

        for (int i = 0; i < numDialogue; i++)
        {
            string rawDialogue = rawTutorialDialogues[i];
            Dialogue parsedDialogue = DialogueParser.Parse(rawDialogue);
            if (parsedDialogue == null)
            {
                Debug.Log("Oops, dialogue (" + rawDialogue + ") is malformed! Stopping generation of tutorial dialogue.");
                break;
            }

            generatedTutorialDialogue[i] = parsedDialogue;
        }
    }

    public static Dialogue GetDialogue(int index)
    {
        if (!hasGenerated)
        {
            GenerateDialogues();
        }
        return dialoguesWithCumulativeChance[index].Item1;
    }

    public static Dialogue GetTutorialDialogue(int index)
    {
        return generatedTutorialDialogue[index];
    }

    public static Dialogue[] GetAllTutorialDialogue()
    {
        return generatedTutorialDialogue;
    }

    public static Dialogue GetRandomDialogue()
    {
        if (!hasGenerated)
        {
            GenerateDialogues();
        }
        // Create new random with time seed
        System.Random rand = new System.Random();
        double randomDouble = rand.NextDouble();

        int numberOfDialogues = dialoguesWithCumulativeChance.Length;
        for (int i = 0; i < numberOfDialogues; i++)
        {
            (Dialogue dialogue, double weight) = dialoguesWithCumulativeChance[i];
            if (randomDouble < weight)
            {
                return dialogue;
            }
        }
        return dialoguesWithCumulativeChance[numberOfDialogues - 1].Item1;
    }
}
