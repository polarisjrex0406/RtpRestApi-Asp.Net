import { useState, useEffect } from 'react';
import { Form, Input, InputNumber, Row, Col, Select } from 'antd';
import TextArea from 'antd/es/input/TextArea';

import { DeleteOutlined } from '@ant-design/icons';
import calculate from '@/utils/calculate';
import useLanguage from '@/locale/useLanguage';
import AutoCompleteAsync from '@/components/AutoCompleteAsync';

import useOnFetch from '@/hooks/useOnFetch';
import { request } from '@/request';

export default function InitPromptItemRow({ field, remove, current = null, curTopicId }) {
    const translate = useLanguage();

    const order = field.key;

    const [selectOptions, setOptions] = useState([]);
    const asyncSearch = async (id) => {
        const entity = 'topic';
        return await request.read({ entity, id });
    };
    let { onFetch, result, isSuccess, isLoading } = useOnFetch();
    useEffect(() => {
        if (curTopicId) {
            const callback = asyncSearch(curTopicId);
            onFetch(callback);
        }
    }, [curTopicId]);

    useEffect(() => {
        if (isSuccess) {
            let filteredRes = [];
            filteredRes = result?.topicPrompt?.map(value => ({ value, label: value }));
            setOptions(filteredRes);
        }
    }, [isSuccess, result]);

    useEffect(() => {
        if (current) {
        }
    }, [current]);

    return (
        <Row gutter={[12, 12]} style={{ position: 'relative' }}>
            <Col className="gutter-row" span={24}>
                <Form.Item
                    name={[field.name]}
                    rules={[
                        {
                            required: true,
                        },
                    ]}
                >
                    <Select
                        options={selectOptions}
                    ></Select>
                </Form.Item>
            </Col>
            <div style={{ position: 'absolute', right: '-20px', top: ' 5px' }}>
                <DeleteOutlined onClick={() => remove(field.name)} />
            </div>
        </Row>
    );
}
