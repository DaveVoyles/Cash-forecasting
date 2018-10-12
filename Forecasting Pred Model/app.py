from flask import Flask
from flask import request
from flask import jsonify
from flask_restful import Resource, Api, reqparse

import numpy as np
import platform
import pandas as pd
from sklearn.preprocessing import LabelEncoder, OneHotEncoder
from sklearn.externals import joblib


app = Flask(__name__)
api = Api(app)

parser = reqparse.RequestParser()

model = None
model_file = './model.joblib'
#forecast_file = '/home/pncdsvm/forecast.csv'

def init():
    global model
    model = joblib.load(model_file)

# Forecast the money withdraw from the ATM
def forecast(forecast_file):
    if model ==None:
        init()
    
    # Import the dataset
    dataset = pd.read_csv(forecast_file)
    X = dataset.iloc[:, 1:10].values
    y = dataset.iloc[:, 10].values
    
    # Encoding categorical data
    from sklearn.preprocessing import LabelEncoder, OneHotEncoder
    labelencoder_X_0 = LabelEncoder()
    labelencoder_X_0.fit(['Mount Road ATM'])
    X[:,0] = labelencoder_X_0.fit_transform(X[:,0])
    labelencoder_X_1 = LabelEncoder()
    labelencoder_X_1.fit(['FRIDAY', 'MONDAY', 'SATURDAY', 'SUNDAY', 'THURSDAY', 'TUESDAY', 'WEDNESDAY'])
    X[:,1] = labelencoder_X_1.fit_transform(X[:, 1])
    labelencoder_X_2 = LabelEncoder()
    labelencoder_X_2.fit(['C', 'H', 'M', 'N', 'NH'])
    X[:,2] = labelencoder_X_2.fit_transform(X[:, 2])
    labelencoder_X_3 = LabelEncoder()
    labelencoder_X_3.fit(['H', 'W'])
    X[:,3] = labelencoder_X_3.fit_transform(X[:, 3])
    labelencoder_X_4 = LabelEncoder()
    labelencoder_X_4.fit(['HHH', 'HHW', 'HWH', 'HWW', 'WHH', 'WHW', 'WWH', 'WWW'])
    X[:,4] = labelencoder_X_4.fit_transform(X[:, 4])
    labelencoder_X_5 = LabelEncoder()
    labelencoder_X_5.fit([1, 2, 3, 4, 5, 6, 7])
    X[:,5] = labelencoder_X_5.fit_transform(X[:, 5])
    
    from sklearn.preprocessing import StandardScaler
    sc = StandardScaler()
    X_forecast = sc.fit_transform(X)
      
    y_forecast = model.predict(X_forecast)
   
    # Form the forecasted results
    for i in range(len(y_forecast)):
        #results += y_forecast[i]
        print("Predicted=%s" % (y_forecast[i]))
        
    return str(y_forecast)

class Forecast(Resource):
    def get(self):
        return {
            'ataturk': {
                'quote': ['Yurtta sulh, cihanda sulh.', 
                    'Egemenlik verilmez, alınır.', 
                    'Hayatta en hakiki mürşit ilimdir.']
            },
            'linus': {
                'quote': ['Talk is cheap. Show me the code.']
            }

        }
    
    def post(self):
        parser.add_argument('filename', type=str)
        args = parser.parse_args()
        filename = args['filename']
        results = forecast(filename)

        return {
            'status': True,
            'filename': 'File: {}, Results: {} '.format(filename, results)
        }


api.add_resource(Forecast, '/forecast')

if __name__ == '__main__':
    app.run(host="0.0.0.0",port=5000)
