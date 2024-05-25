import { Form, Input, Row, Col, Select } from 'antd';
import TextArea from 'antd/es/input/TextArea';
import { DeleteOutlined } from '@ant-design/icons';

export default function ChatGptSettingItemRow({ field, remove, current = null }) {
  return (
    <Row gutter={[12, 12]} style={{ position: 'relative' }}>
      <Col className="gutter-row" span={4}>
        <Form.Item
          name={[field.name, 'setting']}
          rules={[
            {
              required: true,
              message: 'Missing setting',
            },
          ]}
        >
          <Input placeholder="Setting" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={4}>
        <Form.Item name={[field.name, 'valueType']}
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Select
            options={[
              { value: 'text', label: 'text' },
              { value: 'float', label: 'float' },
              { value: 'integer', label: 'integer' },
            ]}
          ></Select>
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={8}>
        <Form.Item
          name={[field.name, 'value']}
          rules={[
            {
              required: true,
              message: 'Missing value',
            },
          ]}
        >
          <Input placeholder="Value" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={4}>
        <Form.Item
          name={[field.name, 'minValue']}
        >
          <Input placeholder="Min Value" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={4}>
        <Form.Item
          name={[field.name, 'maxValue']}
        >
          <Input placeholder="Max Value" />
        </Form.Item>
      </Col>

      <Col className="gutter-row" span={24}>
        <Form.Item
          name={[field.name, 'description']}
        >
          <TextArea placeholder="Description" />
        </Form.Item>
      </Col>

      <div style={{ position: 'absolute', right: '-20px', top: ' 5px' }}>
        <DeleteOutlined onClick={() => remove(field.name)} />
      </div>
    </Row>
  );
}
