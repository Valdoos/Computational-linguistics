import nltk
from nltk.tokenize import RegexpTokenizer
from nltk import pos_tag
from pprint import pprint
from nltk.stem import WordNetLemmatizer 
from nltk.corpus import wordnet
import sys

def get_wordnet_pos(word):
    """Map POS tag to first character lemmatize() accepts"""
    tag = nltk.pos_tag([word])[0][1][0].upper()
    tag_dict = {"J": wordnet.ADJ,
                "N": wordnet.NOUN,
                "V": wordnet.VERB,
                "R": wordnet.ADV}
    return tag_dict.get(tag, wordnet.NOUN)


tokenizer = RegexpTokenizer('[a-zA-Z]+[-]{0,1}[a-zA-Z\']*')

f = open(sys.argv[1],'r',encoding="ISO-8859-1")
data = f.read()
filterdText=tokenizer.tokenize(data)
lemmatizer = WordNetLemmatizer()
tag = [lemmatizer.lemmatize(w,get_wordnet_pos(w)) for w in filterdText]
tokens_tag = pos_tag(filterdText)
str=""
handle = open("output.txt", "w")
for i in range(len(tokens_tag)):
	str=tokens_tag[i][0]+" "+tokens_tag[i][1]+" "+tag[i]+"\n"
	handle.write(str)