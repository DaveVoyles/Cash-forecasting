FROM python:3
RUN apt-get update -y
RUN apt-get install -y python-pip python-dev build-essential && pip install --upgrade pip && pip install numpy && pip install flask && pip install scipy && pip install pandas && pip install scikit-learn &&  pip install requests && pip install flask-restful 
COPY . /app
WORKDIR /app
ENTRYPOINT ["python"]
CMD ["app.py"]