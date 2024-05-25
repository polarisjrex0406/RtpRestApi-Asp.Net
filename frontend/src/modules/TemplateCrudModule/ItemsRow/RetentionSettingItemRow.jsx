import { Form, Input, Row, Col } from 'antd';
import { DeleteOutlined } from '@ant-design/icons';

export default function RetentionSettingItemRow({ field, remove, current = null }) {
  return (
    <Row gutter={[12, 12]} style={{ position: 'relative' }}>
      <Col className="gutter-row" span={8}>
        <Form.Item
          name={[field.name, 'key']}
        >
          <Input placeholder="Key" />
        </Form.Item>
      </Col>
      <Col className="gutter-row" span={16}>
        <Form.Item
          name={[field.name, 'changeDetection']}
        >
          <Input placeholder="Change Detection" />
        </Form.Item>
      </Col>
      <div style={{ position: 'absolute', right: '-20px', top: ' 5px' }}>
        <DeleteOutlined onClick={() => remove(field.name)} />
      </div>
    </Row>
  );
}
