CREATE TABLE MySQLDataTypes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    int_col INT,
    tinyint_col TINYINT,
    smallint_col SMALLINT,
    mediumint_col MEDIUMINT,
    bigint_col BIGINT,
    float_col FLOAT,
    double_col DOUBLE,
    decimal_col DECIMAL(10, 2),
    char_col CHAR(10),
    varchar_col VARCHAR(255),
    text_col TEXT,
    date_col DATE,
    datetime_col DATETIME,
    timestamp_col TIMESTAMP,
    time_col TIME,
    year_col YEAR,
    blob_col BLOB,
    json_col JSON,
    enum_col ENUM('value1', 'value2', 'value3'),
    set_col SET('set1', 'set2', 'set3')
);


INSERT INTO MySQLDataTypes (int_col, tinyint_col, smallint_col, mediumint_col, bigint_col, 
                            float_col, double_col, decimal_col, char_col, varchar_col, 
                            text_col, date_col, datetime_col, timestamp_col, time_col, 
                            year_col, blob_col, json_col, enum_col, set_col) 
VALUES 
(123, 1, 32767, 8388607, 9223372036854775807, 
 3.14, 2.718281828459, 12345.67, 'A', 'Sample Text', 
 'This is a sample text for the TEXT column.', '2023-01-01', 
 '2023-01-01 12:00:00', CURRENT_TIMESTAMP, '12:30:00', 
 2023, UNHEX('48656C6C6F'), '{"key": "value"}', 'value1', 'set1,set2');
