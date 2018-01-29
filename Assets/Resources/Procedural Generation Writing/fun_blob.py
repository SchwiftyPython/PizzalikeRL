from textblob import TextBlob

text = """I was sitting at my computer, bored on a Friday night as usual, so I went on Buzzfeed. I was scrolling past an article about how much of an icon Britney is despite not being relevant for about ten years when I saw a quiz titled: WE CAN GUESS EVERYTHING ABOUT U"
With nothing better to do, I clicked on thw quiz. The first few questions were normal, like "what's your favorite genre of music?" or "what's your nationality?" 
But when I got to the later questi0ns, things started getting spooky. "SELECT YOUR NAME" the question red, and I was shocked to see my own name staring at me, plane as day. I clicked it, not knowing what to do, when I got to the next question: "select your Address" and lo and behold, the address was right there.
But when i got to the third, question, i had a heart attack. "WHO IS THAT OUTSIDE YOUR WINDOW??//?"
I immodesty turned around and saw a dark, humanoid shape standing outside my window, running it's long, thin, spooky fingers down the glass. I called the police and ran into my closet
When they arrived, they investigated outside and inside, but no one was there. "We'll send a car by soon" the officer said before leaving
I decided to get a bowl of cereal to calm my nerves. But when I tipped the box, a cascade of shredded CD's floated down into the bowl, followed by a note. Right after I read it, I had a fatal heart attack:
"BLACKOUT IS BRITNEY'S BEST ALBUM AND i will FIGHT you For It" """

blob = TextBlob(text)

print(blob.tags)
