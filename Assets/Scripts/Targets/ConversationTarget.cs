using UnityEngine;

[System.Serializable]
public class Conversation
{
    [TextArea(3, 10)]
    public string[] lines;
}

public class ConversationTarget : MonoBehaviour
{
    private Conversation[] conversations;
    string cachedConversation = "";

    void Awake()
    {
        conversations = BuildConversations();
    }

    Conversation[] BuildConversations()
    {
        return new Conversation[100]
        {
            new Conversation { lines = new string[] { "Person 1: Did you hear that bird?", "Person 2: Yes, and I think it heard us first.", "Person 1: Great. I am being judged by a pigeon." } },
            new Conversation { lines = new string[] { "Person 1: This bench feels haunted.", "Person 2: That is just because you sat on gum.", "Person 1: Somehow that is worse." } },
            new Conversation { lines = new string[] { "Person 1: That bird has been staring at us for ten minutes.", "Person 2: Maybe you owe it money.", "Person 1: I do not remember borrowing from birds." } },
            new Conversation { lines = new string[] { "Person 1: I came here for peace and quiet.", "Person 2: Then why did you bring a leaf blower smoothie cup?", "Person 1: It is called blending on the go." } },
            new Conversation { lines = new string[] { "Person 1: Do squirrels have a government?", "Person 2: If they do, it is definitely corrupt.", "Person 1: That explains the acorn taxes." } },
            new Conversation { lines = new string[] { "Person 1: That surveillance bird blinked at me.", "Person 2: Stay calm. It can smell fear.", "Person 1: Birds cannot smell fear.", "Person 2: This one has funding." } },
            new Conversation { lines = new string[] { "Person 1: I tried jogging this morning.", "Person 2: How far did you get?", "Person 1: Emotionally? Not very." } },
            new Conversation { lines = new string[] { "Person 1: Why is that duck walking like it pays rent here?", "Person 2: Because it probably does.", "Person 1: Then I want to speak to management." } },
            new Conversation { lines = new string[] { "Person 1: I think the birds know my schedule.", "Person 2: You do feed them every day at noon.", "Person 1: So I trained my own informants." } },
            new Conversation { lines = new string[] { "Person 1: This park is beautiful.", "Person 2: Yes, except for that goose with villain energy.", "Person 1: He does walk like he has a monologue." } },
            new Conversation { lines = new string[] { "Person 1: You ever feel like nature is watching?", "Person 2: That bird is literally facing us.", "Person 1: I hope my good side is my left side." } },
            new Conversation { lines = new string[] { "Person 1: I brought bread for the ducks.", "Person 2: You know they say not to do that.", "Person 1: Then I brought plausible deniability for the ducks." } },
            new Conversation { lines = new string[] { "Person 1: Why is that bird so still?", "Person 2: Because movement wastes battery.", "Person 1: Oh good, we skipped paranoia and went straight to certainty." } },
            new Conversation { lines = new string[] { "Person 1: My phone says I walked three miles.", "Person 2: Mine says I need a nap.", "Person 1: Yours is the more honest device." } },
            new Conversation { lines = new string[] { "Person 1: That kid just called me old.", "Person 2: What did you do?", "Person 1: I made a sound when I sat down." } },
            new Conversation { lines = new string[] { "Person 1: The bird looked at my sandwich.", "Person 2: That is because you opened it like a flare.", "Person 1: So this is on me." } },
            new Conversation { lines = new string[] { "Person 1: I came here to clear my mind.", "Person 2: Did it work?", "Person 1: No, now it is just windy in there." } },
            new Conversation { lines = new string[] { "Person 1: That squirrel climbed into the trash can.", "Person 2: Resourceful.", "Person 1: Criminal, but resourceful." } },
            new Conversation { lines = new string[] { "Person 1: Why are park maps always so confident?", "Person 2: Because unlike us, they know where they are.", "Person 1: Rude and fair." } },
            new Conversation { lines = new string[] { "Person 1: That bird landed right next to us.", "Person 2: Act natural.", "Person 1: I forgot how to be a person under pressure." } },
            new Conversation { lines = new string[] { "Person 1: I packed healthy snacks.", "Person 2: Then why are you eating kettle corn?", "Person 1: Emotionally, it is a vegetable." } },
            new Conversation { lines = new string[] { "Person 1: The breeze is nice today.", "Person 2: That is pollen.", "Person 1: Then my allergies are about to write a complaint." } },
            new Conversation { lines = new string[] { "Person 1: I think that bird is listening to us.", "Person 2: Good. Maybe it can explain your plan back to me.", "Person 1: Betrayed by wildlife." } },
            new Conversation { lines = new string[] { "Person 1: Why do dogs always look busier than people here?", "Person 2: Because they have purpose.", "Person 1: Mine is mostly surviving Mondays." } },
            new Conversation { lines = new string[] { "Person 1: That goose hissed at me.", "Person 2: What did you do?", "Person 1: Existed too close to it, apparently." } },
            new Conversation { lines = new string[] { "Person 1: I waved at that bird by accident.", "Person 2: Now it knows you are onto it.", "Person 1: Excellent. I have challenged the sky." } },
            new Conversation { lines = new string[] { "Person 1: I love people watching in parks.", "Person 2: Pretty sure that bird loves it more.", "Person 1: It does have the better seat." } },
            new Conversation { lines = new string[] { "Person 1: I tried meditation out here.", "Person 2: How did it go?", "Person 1: A duck interrupted my inner peace for a cracker." } },
            new Conversation { lines = new string[] { "Person 1: That bird tilted its head at me.", "Person 2: It is either curious or scanning.", "Person 1: Neither option helps me relax." } },
            new Conversation { lines = new string[] { "Person 1: Why are park benches designed to make you question your spine?", "Person 2: To keep you from staying too long.", "Person 1: Then the bench has won." } },
            new Conversation { lines = new string[] { "Person 1: I think I stepped in mud.", "Person 2: That is the optimistic interpretation.", "Person 1: I am choosing optimism and walking away." } },
            new Conversation { lines = new string[] { "Person 1: That bird keeps following us.", "Person 2: Maybe we are interesting.", "Person 1: Then we should apologize for disappointing it." } },
            new Conversation { lines = new string[] { "Person 1: I brought a book to read in the park.", "Person 2: Nice. What chapter are you on?", "Person 1: Still page one. I got distracted by a dramatic squirrel." } },
            new Conversation { lines = new string[] { "Person 1: Why do kids have endless energy?", "Person 2: Because they have never paid taxes.", "Person 1: That checks out." } },
            new Conversation { lines = new string[] { "Person 1: That bird landed on the sign.", "Person 2: Elevated position. Better acoustics for spying.", "Person 1: I hate how ready you were with that." } },
            new Conversation { lines = new string[] { "Person 1: I wanted a peaceful walk.", "Person 2: And instead?", "Person 1: I got cardio and emotional growth." } },
            new Conversation { lines = new string[] { "Person 1: This duck is judging my posture.", "Person 2: Fix it then.", "Person 1: I will not be corrected by a water bird." } },
            new Conversation { lines = new string[] { "Person 1: That surveillance bird has excellent timing.", "Person 2: It arrived right when the gossip did.", "Person 1: Honestly, same." } },
            new Conversation { lines = new string[] { "Person 1: Do you think trees get tired of us?", "Person 2: Probably, but they are taking it well.", "Person 1: Strong role models, the trees." } },
            new Conversation { lines = new string[] { "Person 1: Why is everyone in the park either running or thriving?", "Person 2: Some people contain multitudes.", "Person 1: I contain snacks." } },
            new Conversation { lines = new string[] { "Person 1: I heard flapping behind us.", "Person 2: Do not look now.", "Person 1: Why?", "Person 2: Because if it is the bird, I want plausible ignorance." } },
            new Conversation { lines = new string[] { "Person 1: This coffee tastes like ambition.", "Person 2: Mine tastes like burnt regret.", "Person 1: So we went to the same place." } },
            new Conversation { lines = new string[] { "Person 1: The bird moved when I mentioned secrets.", "Person 2: That is not comforting.", "Person 1: Neither is the tiny antenna." } },
            new Conversation { lines = new string[] { "Person 1: I forgot sunscreen.", "Person 2: You remembered complaining though.", "Person 1: I never forget what matters." } },
            new Conversation { lines = new string[] { "Person 1: That squirrel just stole a whole cookie.", "Person 2: And honestly, I respect the confidence.", "Person 1: It did commit the perfect crime." } },
            new Conversation { lines = new string[] { "Person 1: Why does that bird keep circling?", "Person 2: Maybe it is gathering evidence.", "Person 1: Against what, my lunch choices?" } },
            new Conversation { lines = new string[] { "Person 1: I tried to identify a bird species today.", "Person 2: How did that go?", "Person 1: I confidently invented three birds." } },
            new Conversation { lines = new string[] { "Person 1: The park is crowded today.", "Person 2: Good. More witnesses when the geese strike.", "Person 1: Comforting in a legal sense only." } },
            new Conversation { lines = new string[] { "Person 1: That bird looked right at me when I lied.", "Person 2: So now nature is doing fact checks.", "Person 1: We used to have privacy." } },
            new Conversation { lines = new string[] { "Person 1: I packed grapes for the picnic.", "Person 2: Fancy.", "Person 1: They were on sale. Let us not get ahead of ourselves." } },
            new Conversation { lines = new string[] { "Person 1: Why is that man sprinting like the park is on fire?", "Person 2: Fitness.", "Person 1: Could not be me. I run only for dramatic reasons." } },
            new Conversation { lines = new string[] { "Person 1: The bird blinked twice.", "Person 2: Maybe that is a code.", "Person 1: I do not want to be fluent in bird espionage." } },
            new Conversation { lines = new string[] { "Person 1: I like sitting near the pond.", "Person 2: Very peaceful.", "Person 1: Except for the ducks negotiating aggressively." } },
            new Conversation { lines = new string[] { "Person 1: I think my sandwich attracted a crow.", "Person 2: That is not your sandwich anymore.", "Person 1: I see. Park law." } },
            new Conversation { lines = new string[] { "Person 1: That bird has better posture than I do.", "Person 2: It also has stronger boundaries.", "Person 1: This park is full of role models." } },
            new Conversation { lines = new string[] { "Person 1: I came here to touch grass.", "Person 2: Did it help?", "Person 1: Emotionally no, but the grass seemed supportive." } },
            new Conversation { lines = new string[] { "Person 1: Why are there always joggers before sunrise?", "Person 2: To remind the rest of us of our failures.", "Person 1: Ah. Community outreach." } },
            new Conversation { lines = new string[] { "Person 1: That bird landed on the lamp post again.", "Person 2: High ground. Classic surveillance behavior.", "Person 1: Stop making it sound competent." } },
            new Conversation { lines = new string[] { "Person 1: This pretzel is stale.", "Person 2: So is your attitude.", "Person 1: That is fair and unrelated." } },
            new Conversation { lines = new string[] { "Person 1: I dropped one chip and got surrounded by pigeons.", "Person 2: You triggered an event.", "Person 1: I did not know snacks had boss fights." } },
            new Conversation { lines = new string[] { "Person 1: The bird followed us to a second bench.", "Person 2: That means either devotion or surveillance.", "Person 1: I dislike both." } },
            new Conversation { lines = new string[] { "Person 1: I wore new shoes to the park.", "Person 2: Brave choice.", "Person 1: My heels and I no longer speak." } },
            new Conversation { lines = new string[] { "Person 1: Why is there always one person doing yoga near the fountain?", "Person 2: Balance.", "Person 1: I can barely do standing." } },
            new Conversation { lines = new string[] { "Person 1: That bird was on the statue earlier.", "Person 2: Observation point.", "Person 1: Everything is an observation point when you are committed enough." } },
            new Conversation { lines = new string[] { "Person 1: I said I wanted fresh air.", "Person 2: And now?", "Person 1: Now I have fresh pollen and several questions." } },
            new Conversation { lines = new string[] { "Person 1: The ducks seem organized today.", "Person 2: Never trust coordinated ducks.", "Person 1: Finally, advice I can use." } },
            new Conversation { lines = new string[] { "Person 1: That bird is closer than before.", "Person 2: It is probably trying to hear better.", "Person 1: Then let us discuss boring taxes." } },
            new Conversation { lines = new string[] { "Person 1: I brought a frisbee.", "Person 2: Great. Any athletic ability?", "Person 1: Absolutely none. This is mostly optimism." } },
            new Conversation { lines = new string[] { "Person 1: That goose has not moved.", "Person 2: It does not need to. Fear travels.", "Person 1: Remarkable efficiency." } },
            new Conversation { lines = new string[] { "Person 1: The bird left when we stopped gossiping.", "Person 2: So it really was listening.", "Person 1: We are content creators for pigeons." } },
            new Conversation { lines = new string[] { "Person 1: I sat on this bench for one minute and already need a snack.", "Person 2: That is not park hunger. That is just you.", "Person 1: Correct." } },
            new Conversation { lines = new string[] { "Person 1: Why do dogs in bandanas look more successful than me?", "Person 2: Branding.", "Person 1: I knew I forgot something." } },
            new Conversation { lines = new string[] { "Person 1: That bird has been above us the whole walk.", "Person 2: Maybe it likes your hat.", "Person 1: Or my confidential aura." } },
            new Conversation { lines = new string[] { "Person 1: I cannot tell if that is a robin or just a red leaf with ambition.", "Person 2: Nature keeps some mysteries.", "Person 1: And I keep being wrong." } },
            new Conversation { lines = new string[] { "Person 1: The pond smells kind of weird.", "Person 2: That is called authenticity.", "Person 1: Then authenticity needs a filter." } },
            new Conversation { lines = new string[] { "Person 1: That bird hopped closer when I mentioned my boss.", "Person 2: Even birds love workplace drama.", "Person 1: We truly are universal." } },
            new Conversation { lines = new string[] { "Person 1: I thought today would be relaxing.", "Person 2: Then you saw the goose.", "Person 1: My peace now has an opponent." } },
            new Conversation { lines = new string[] { "Person 1: Why are park squirrels either adorable or criminal?", "Person 2: Range.", "Person 1: Impressive range." } },
            new Conversation { lines = new string[] { "Person 1: The bird landed on the trash can.", "Person 2: Tactical and symbolic.", "Person 1: That sentence upset me in two ways." } },
            new Conversation { lines = new string[] { "Person 1: I packed sparkling water for the picnic.", "Person 2: Fancy.", "Person 1: It was the same price as regular and I panicked." } },
            new Conversation { lines = new string[] { "Person 1: That little bird is pretending not to listen.", "Person 2: Very professional of it.", "Person 1: Better poker face than most people I know." } },
            new Conversation { lines = new string[] { "Person 1: I like this path. It feels scenic.", "Person 2: It also feels uphill.", "Person 1: Beauty demands sacrifice." } },
            new Conversation { lines = new string[] { "Person 1: Did you bring bug spray?", "Person 2: No.", "Person 1: Then tonight we donate blood to the park." } },
            new Conversation { lines = new string[] { "Person 1: That bird keeps reappearing.", "Person 2: Maybe it has a route.", "Person 1: Great, we are on a patrol schedule." } },
            new Conversation { lines = new string[] { "Person 1: I wanted to have a deep conversation out here.", "Person 2: We can still do that.", "Person 1: Not with a pigeon wearing that expression." } },
            new Conversation { lines = new string[] { "Person 1: Why is the ice cream truck in the park so loud?", "Person 2: To awaken your inner child.", "Person 1: Mine wants a refund." } },
            new Conversation { lines = new string[] { "Person 1: That bird looked offended.", "Person 2: Maybe it did not like your tone.", "Person 1: Then it should stop eavesdropping professionally." } },
            new Conversation { lines = new string[] { "Person 1: I brought trail mix.", "Person 2: That is just tiny chores in a bag.", "Person 1: Delicious chores." } },
            new Conversation { lines = new string[] { "Person 1: The dog over there is living a better life than I am.", "Person 2: That dog has a tennis ball and no inbox.", "Person 1: Exactly my point." } },
            new Conversation { lines = new string[] { "Person 1: Why is that bird on the fence again?", "Person 2: Better line of sight.", "Person 1: You keep saying things that make me uncomfortable." } },
            new Conversation { lines = new string[] { "Person 1: I came to the park to save money.", "Person 2: By doing what?", "Person 1: Existing near expensive coffee without buying it." } },
            new Conversation { lines = new string[] { "Person 1: The breeze just stole my napkin.", "Person 2: Nature collects dues.", "Person 1: It could have billed me instead." } },
            new Conversation { lines = new string[] { "Person 1: That bird only shows up when we talk nonsense.", "Person 2: So it has been well fed.", "Person 1: Cruel and accurate." } },
            new Conversation { lines = new string[] { "Person 1: Why do ducks always look like they know a secret?", "Person 2: Because they live in ponds and avoid taxes.", "Person 1: Mystical." } },
            new Conversation { lines = new string[] { "Person 1: I stepped on a twig and scared myself.", "Person 2: Survival instincts still work.", "Person 1: Embarrassingly well." } },
            new Conversation { lines = new string[] { "Person 1: That bird has been at every tree we passed.", "Person 2: Consistency is important in surveillance.", "Person 1: I liked birds more when they seemed random." } },
            new Conversation { lines = new string[] { "Person 1: I planned a calm picnic.", "Person 2: Then why did you invite geese by opening chips?", "Person 1: I forgot chips are a public announcement." } },
            new Conversation { lines = new string[] { "Person 1: The clouds look nice today.", "Person 2: Yes, very dramatic.", "Person 1: Even the sky is doing theater." } },
            new Conversation { lines = new string[] { "Person 1: That bird turned its whole body to face us.", "Person 2: Very efficient. Very unsettling.", "Person 1: We are definitely in somebody's report." } },
            new Conversation { lines = new string[] { "Person 1: Do you think birds gossip about us?", "Person 2: Absolutely.", "Person 1: Then I hope my rumors are flattering." } }
        };
    }

    public string GetConversationText()
    {
        if (!string.IsNullOrEmpty(cachedConversation))
            return cachedConversation;

        if (conversations == null || conversations.Length == 0)
            return "";

        int validCount = 0;

        for (int i = 0; i < conversations.Length; i++)
        {
            if (conversations[i] != null && conversations[i].lines != null && conversations[i].lines.Length > 0)
                validCount++;
        }

        if (validCount == 0)
            return "";

        int chosenValid = Random.Range(0, validCount);
        int currentValid = 0;
        Conversation chosenConversation = null;

        for (int i = 0; i < conversations.Length; i++)
        {
            if (conversations[i] != null && conversations[i].lines != null && conversations[i].lines.Length > 0)
            {
                if (currentValid == chosenValid)
                {
                    chosenConversation = conversations[i];
                    break;
                }

                currentValid++;
            }
        }

        if (chosenConversation == null)
            return "";

        string[] lines = (string[])chosenConversation.lines.Clone();
        string thisName = gameObject.name;
        string otherName = GetNearestOtherTargetName();

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;

            lines[i] = lines[i].Replace("Person 1", thisName)
                               .Replace("Person 2", otherName);
        }

        cachedConversation = string.Join("\n", lines);
        return cachedConversation;
    }

    public void ClearCachedConversation()
    {
        cachedConversation = "";
    }

    string GetNearestOtherTargetName()
    {
        ConversationTarget[] allTargets = FindObjectsByType<ConversationTarget>(FindObjectsSortMode.None);
        ConversationTarget nearestTarget = null;
        float nearestSqrDistance = float.MaxValue;
        Vector3 myPosition = transform.position;

        for (int i = 0; i < allTargets.Length; i++)
        {
            ConversationTarget other = allTargets[i];
            if (other == null || other == this) continue;

            float sqrDistance = (other.transform.position - myPosition).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestTarget = other;
            }
        }

        return nearestTarget != null ? nearestTarget.gameObject.name : "Unknown";
    }
}