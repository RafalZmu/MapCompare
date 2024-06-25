from gpt4all import GPT4All
import pandas as pd

BigModel = "mistral-7b-instruct-v0.1.Q4_0.gguf"
modelPath =r"D:\Projekty\Praca_inz\MapCompare\MapCompereAPI\GPT4AllService\Models"

SmallModel = "orca-mini-3b-gguf2-q4_0.gguf"

system_prompt = '### System:\nYou are a data scientist. You will be given csv file and you need to extract essential information from it get the example column names. The data will be related to countires. Extract what the data is about'

example_data_path = r"C:\Users\rafal\Downloads\SYB66_1_202310_Population, Surface Area and Density.csv"

df = pd.read_csv(example_data_path, encoding='iso-8859-1')
columns = df.columns.tolist()

model = GPT4All(model_name=BigModel, model_path=modelPath, device="gpu")

with model.chat_session(system_prompt=system_prompt):
    prompt = ""
    prompt += "\n".join([", ".join(column) for column in columns]) 
    output = model.generate(prompt, max_tokens=5000, temp=0, )
    print(output)