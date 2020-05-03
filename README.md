# XTextCategorizer.Demo
This project is the demo for the XTextCategorizer library.

# XTextCategorizer
You can find the XTextCategorizer library on NuGet: https://www.nuget.org/packages/XTextCategorizer
## Overview
XTextCategorizer is a binary, supervised learning text classifier, using Convolutional Neural Network architecture
## Usage
1) Creates a new instance of TextCategorizer
2) Calls Train(samples) to train the text categorizer
3) Calls Predict(samples) to make prediction
## Save and Load
In the class TextCategorizer, after training, you can use the method SaveAsString() will save the current state of the text categorizer as a string.  
The method LoadFromString(state) will restore the state of the TextCategorizer  
You can also use the method SaveToFile(path) and LoadFromFile(path) to save/load from file 
## Demo Result
Dataset: Large Movie Review Dataset, in courtesy of https://ai.stanford.edu/~amaas/data/sentiment/
| Metrics | Favoring Speed mode | Favoring Accuracy mode |
| --- | --- | --- |
| Accuracy | 57.5% | 80.2% |
| Precision | 56.8% | 80.2% |
| Recall | 62.7% | 80.2% |
| F1 Score | 59.6 | 80.2 |
## Contact
For any question please send an email to rchristengroup@gmail.com
