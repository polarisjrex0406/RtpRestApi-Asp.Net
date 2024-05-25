import { Form, Input, Col, Select } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';

export default function ConditionsItemRow({ field, remove, current = null }) {
  return (
    <>
      <Col className="gutter-row" span={4}>
        <Form.Item
          name={[field.name, 'conditionName']}
        >
          <Input placeholder="Condition Name" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={5}>
        <Form.Item name={[field.name, 'conditionType']}
        >
          <Select
            options={[
              { value: 'lastresponse', label: 'Last Response' },
              { value: 'topicprompt', label: 'Topic Prompt' },
              { value: 'key', label: 'Key' },
            ]}
          ></Select>
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={5}>
        <Form.Item
          name={[field.name, 'conditionItem']}
        >
          <Input placeholder="Condition Item" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={3}>
        <Form.Item
          name={[field.name, 'conditionOperator']}
        >
          <Select
            options={[
              { value: 'EQ', label: 'EQ' },
              { value: 'NEQ', label: 'NEQ' },
              { value: 'LT', label: 'LT' },
              { value: 'LTE', label: 'LTE' },
              { value: 'GT', label: 'GT' },
              { value: 'GTE', label: 'GTE' },
              { value: 'IN', label: 'IN' },
              { value: 'NOTIN', label: 'NOTIN' },
            ]}
          ></Select>
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={6}>
        <Form.Item
          name={[field.name, 'conditionValue']}
        >
          <Input placeholder="Condition Value" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={1}>
        <DeleteOutlined onClick={() => remove(field.name)} />
      </Col>
    </>
  );
}
